/*
================================================================================
CORRECCIÓN: Trigger con recursividad infinita
================================================================================

ERROR ANTERIOR:
"Maximum stored procedure, function, trigger, or view nesting level exceeded (limit 32)"

CAUSA:
El trigger se dispara en UPDATE, y dentro hace otro UPDATE a la misma tabla,
causando un loop infinito.

SOLUCIÓN:
Agregar condición IF UPDATE(LATITUD) OR UPDATE(LONGITUD) para que solo 
se ejecute cuando cambian las coordenadas, no cuando cambia PUNTO_GEOGRAFICO.

================================================================================
*/

USE bdMiBus;
GO

-- Eliminar el trigger con problema
IF OBJECT_ID('dbo.TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO', 'TR') IS NOT NULL
BEGIN
    DROP TRIGGER dbo.TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO;
    PRINT '? Trigger anterior eliminado';
END
GO

-- Crear el trigger CORREGIDO (sin recursividad)
CREATE TRIGGER TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO
ON TB_PARADA
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- CLAVE: Solo ejecutar si LATITUD o LONGITUD cambiaron (evita recursividad)
    -- NOT EXISTS(SELECT 1 FROM deleted) = es un INSERT (no hay fila anterior)
    IF UPDATE(LATITUD) OR UPDATE(LONGITUD) OR NOT EXISTS(SELECT 1 FROM deleted)
    BEGIN
        UPDATE TB_PARADA
        SET PUNTO_GEOGRAFICO = geography::Point(i.LATITUD, i.LONGITUD, 4326)
        FROM TB_PARADA p
        INNER JOIN inserted i ON p.GUID_PARADA = i.GUID_PARADA
        WHERE i.LATITUD IS NOT NULL 
          AND i.LONGITUD IS NOT NULL;
    END
END
GO

PRINT '? Trigger TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO creado correctamente (SIN recursividad)';
GO

-- Actualizar datos existentes (si los hay)
UPDATE TB_PARADA
SET PUNTO_GEOGRAFICO = geography::Point(LATITUD, LONGITUD, 4326)
WHERE LATITUD IS NOT NULL 
  AND LONGITUD IS NOT NULL
  AND PUNTO_GEOGRAFICO IS NULL;

DECLARE @Actualizados INT = @@ROWCOUNT;
IF @Actualizados > 0
    PRINT '? ' + CAST(@Actualizados AS VARCHAR) + ' registro(s) actualizado(s)';
ELSE
    PRINT '? No hay registros pendientes de actualizar';
GO

PRINT '';
PRINT '========================================';
PRINT '? TRIGGER CORREGIDO EXITOSAMENTE';
PRINT '========================================';
PRINT '';
PRINT 'Ahora puedes:';
PRINT '1. Insertar paradas desde C#';
PRINT '2. Actualizar LATITUD/LONGITUD sin problemas';
PRINT '3. PUNTO_GEOGRAFICO se calculará automáticamente';
PRINT '';
GO

-- PRUEBA (ejecuta esta parte para verificar que funciona)
PRINT '========================================';
PRINT 'PRUEBA: Insertar parada de ejemplo';
PRINT '========================================';

-- Limpiar prueba anterior si existe
DELETE FROM TB_PARADA WHERE NOMBRE = 'Parada Test Trigger';

-- Insertar prueba
INSERT INTO TB_PARADA (GUID_PARADA, NOMBRE, DESCRIPCION, LATITUD, LONGITUD, ESTADO, FECHA_REGISTRO)
VALUES (NEWID(), 'Parada Test Trigger', 'Prueba del trigger corregido', 9.9281, -84.0907, 1, GETDATE());

PRINT '? Parada insertada';

-- Verificar resultado
SELECT TOP 1
    NOMBRE,
    LATITUD,
    LONGITUD,
    PUNTO_GEOGRAFICO.Lat as Lat_Calculada,
    PUNTO_GEOGRAFICO.Long as Long_Calculada,
    CASE 
        WHEN PUNTO_GEOGRAFICO IS NOT NULL THEN '? OK - Trigger funcionó'
        ELSE '? ERROR - Trigger NO funcionó'
    END as Estado
FROM TB_PARADA
WHERE NOMBRE = 'Parada Test Trigger';

PRINT '';
PRINT '? Si ves "? OK - Trigger funcionó", todo está perfecto!';
PRINT '';
GO
