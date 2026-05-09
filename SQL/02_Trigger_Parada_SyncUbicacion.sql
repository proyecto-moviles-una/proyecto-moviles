/*
================================================================================
TRIGGER: TR_TB_PARADA_SYNC_UBICACION
================================================================================

PROPÓSITO:
----------
Sincroniza automáticamente el campo UBICACION (geography) cuando se insertan
o actualizan registros en la tabla TB_PARADA.

¿POR QUÉ ES NECESARIO ESTE TRIGGER?
------------------------------------
LINQ to SQL no soporta el tipo de dato GEOGRAPHY de SQL Server.

Problema:
- La tabla TB_PARADA tiene un campo UBICACION de tipo geography
- LINQ to SQL no puede mapear este tipo de dato
- Visual Studio no permite arrastrar la tabla al diseñador DBML

Solución:
1. En C# (LINQ to SQL): Solo trabajamos con LATITUD y LONGITUD (double)
2. En SQL Server: Este trigger calcula automáticamente UBICACION usando geography::Point()
3. Resultado: El código C# es simple y el campo geography se mantiene actualizado

FUNCIONAMIENTO:
---------------
1. Cuando se hace INSERT o UPDATE en TB_PARADA
2. El trigger se ejecuta DESPUÉS de la operación (AFTER)
3. Calcula: UBICACION = geography::Point(LATITUD, LONGITUD, 4326)
   - 4326 = SRID (Spatial Reference ID) para WGS84 (sistema GPS estándar)
   - geography::Point(lat, long, srid) crea un punto geográfico

VENTAJAS:
---------
? El código C# solo maneja coordenadas simples (LATITUD, LONGITUD)
? El campo UBICACION siempre está sincronizado
? Podemos usar funciones espaciales de SQL Server (STDistance, STWithin, etc.)
? No necesitamos código adicional en C# para calcular geography

EJEMPLO DE USO EN C#:
---------------------
TB_PARADA nueva = new TB_PARADA
{
    LATITUD = 9.9281,   // Solo asignamos lat/long
    LONGITUD = -84.0907
};
db.TB_PARADAs.InsertOnSubmit(nueva);
db.SubmitChanges();
// ? El trigger automáticamente calcula y guarda UBICACION

CONSULTAS ESPACIALES (EJEMPLOS):
---------------------------------
-- Paradas dentro de 1km de un punto:
SELECT * FROM TB_PARADA
WHERE UBICACION.STDistance(geography::Point(9.9281, -84.0907, 4326)) <= 1000;

-- Distancia entre dos paradas:
SELECT p1.NOMBRE, p2.NOMBRE, 
       p1.UBICACION.STDistance(p2.UBICACION) as DISTANCIA_METROS
FROM TB_PARADA p1, TB_PARADA p2;

NOTAS:
------
- El trigger solo actualiza registros donde LATITUD y LONGITUD NO son NULL
- Si necesitas actualizar solo un campo (ej: NOMBRE), el trigger NO recalcula
  UBICACION innecesariamente (solo si LATITUD o LONGITUD cambian)

FECHA CREACIÓN: [Tu fecha aquí]
AUTOR: [Tu nombre aquí]
================================================================================
*/

CREATE TRIGGER TR_TB_PARADA_SYNC_UBICACION
ON TB_PARADA
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Actualiza el campo UBICACION (geography) usando las coordenadas LATITUD y LONGITUD
    -- geography::Point(latitud, longitud, 4326) crea un punto geográfico
    -- 4326 = SRID para WGS84 (sistema de coordenadas GPS estándar)
    UPDATE TB_PARADA
    SET UBICACION = geography::Point(i.LATITUD, i.LONGITUD, 4326)
    FROM TB_PARADA p
    INNER JOIN inserted i ON p.GUID_PARADA = i.GUID_PARADA
    WHERE i.LATITUD IS NOT NULL 
      AND i.LONGITUD IS NOT NULL;
END
GO
