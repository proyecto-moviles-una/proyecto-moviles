/*
================================================================================
SCRIPT MAESTRO: Configuración completa para TB_PARADA con Geography
================================================================================

EJECUCIÓN:
----------
Ejecuta este script completo en SQL Server Management Studio sobre la base 
de datos bdMiBus.

Este script ejecuta en orden:
1. Verifica la columna PUNTO_GEOGRAFICO (geography) en TB_PARADA
2. Crea el trigger para sincronizar automáticamente PUNTO_GEOGRAFICO
3. Actualiza datos existentes (si los hay)

NOTA: Tu tabla ya tiene la columna PUNTO_GEOGRAFICO (tipo geography)
================================================================================
*/

USE bdMiBus;
GO

PRINT '========================================';
PRINT 'PASO 1: Verificar columna PUNTO_GEOGRAFICO';
PRINT '========================================';

-- Verificar si la columna PUNTO_GEOGRAFICO ya existe
IF EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.TB_PARADA') 
    AND name = 'PUNTO_GEOGRAFICO'
)
BEGIN
    PRINT '? La columna PUNTO_GEOGRAFICO ya existe en la tabla';
END
ELSE
BEGIN
    -- Si no existe, crearla
    ALTER TABLE dbo.TB_PARADA
    ADD PUNTO_GEOGRAFICO geography NULL;

    PRINT '? Columna PUNTO_GEOGRAFICO agregada exitosamente';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'PASO 2: Crear trigger de sincronización';
PRINT '========================================';

-- Eliminar el trigger si ya existe (para recrearlo)
IF OBJECT_ID('dbo.TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO', 'TR') IS NOT NULL
BEGIN
    DROP TRIGGER dbo.TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO;
    PRINT '! Trigger anterior eliminado';
END
GO

-- Crear el trigger (sin recursividad)
CREATE TRIGGER TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO
ON TB_PARADA
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Solo ejecutar si LATITUD o LONGITUD cambiaron (evita recursividad)
    IF UPDATE(LATITUD) OR UPDATE(LONGITUD) OR NOT EXISTS(SELECT 1 FROM deleted)
    BEGIN
        -- Actualiza el campo PUNTO_GEOGRAFICO (geography) usando las coordenadas LATITUD y LONGITUD
        -- geography::Point(latitud, longitud, 4326) crea un punto geográfico
        -- 4326 = SRID para WGS84 (sistema de coordenadas GPS estándar)
        UPDATE TB_PARADA
        SET PUNTO_GEOGRAFICO = geography::Point(i.LATITUD, i.LONGITUD, 4326)
        FROM TB_PARADA p
        INNER JOIN inserted i ON p.GUID_PARADA = i.GUID_PARADA
        WHERE i.LATITUD IS NOT NULL 
          AND i.LONGITUD IS NOT NULL;
    END
END
GO

PRINT '? Trigger TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO creado exitosamente';
GO

PRINT '';
PRINT '========================================';
PRINT 'PASO 3: Actualizar datos existentes (si hay)';
PRINT '========================================';

-- Actualizar registros existentes que tienen LATITUD y LONGITUD pero no PUNTO_GEOGRAFICO
DECLARE @RegistrosActualizados INT;

UPDATE TB_PARADA
SET PUNTO_GEOGRAFICO = geography::Point(LATITUD, LONGITUD, 4326)
WHERE LATITUD IS NOT NULL 
  AND LONGITUD IS NOT NULL
  AND PUNTO_GEOGRAFICO IS NULL;

SET @RegistrosActualizados = @@ROWCOUNT;

IF @RegistrosActualizados > 0
    PRINT '? ' + CAST(@RegistrosActualizados AS VARCHAR) + ' registro(s) existente(s) actualizado(s)';
ELSE
    PRINT '! No hay registros existentes para actualizar';
GO

PRINT '';
PRINT '========================================';
PRINT '? CONFIGURACIÓN COMPLETADA EXITOSAMENTE';
PRINT '========================================';
PRINT '';
PRINT 'Ahora puedes:';
PRINT '- Insertar paradas desde C# (el trigger calculará PUNTO_GEOGRAFICO automáticamente)';
PRINT '- Usar consultas espaciales de SQL Server (STDistance, STWithin, etc.)';
PRINT '';
GO

-- PRUEBA (OPCIONAL): Descomentar para probar
/*
PRINT '';
PRINT '========================================';
PRINT 'PRUEBA: Insertar una parada de ejemplo';
PRINT '========================================';

INSERT INTO TB_PARADA (GUID_PARADA, NOMBRE, DESCRIPCION, LATITUD, LONGITUD, ESTADO, FECHA_REGISTRO)
VALUES (
    NEWID(),
    'Parada de Prueba',
    'Esta es una parada de prueba para verificar el trigger',
    9.9281,   -- Latitud (San José, Costa Rica)
    -84.0907, -- Longitud
    1,
    GETDATE()
);

PRINT '? Parada de prueba insertada';

-- Verificar que PUNTO_GEOGRAFICO se calculó automáticamente
SELECT TOP 1
    NOMBRE,
    LATITUD,
    LONGITUD,
    PUNTO_GEOGRAFICO.Lat as UbicacionLatitud,
    PUNTO_GEOGRAFICO.Long as UbicacionLongitud,
    CASE WHEN PUNTO_GEOGRAFICO IS NOT NULL THEN '? OK' ELSE '? ERROR' END as Estado
FROM TB_PARADA
ORDER BY FECHA_REGISTRO DESC;

PRINT '';
*/
