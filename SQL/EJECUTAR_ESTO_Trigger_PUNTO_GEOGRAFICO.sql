/*
================================================================================
TRIGGER: Sincronizar PUNTO_GEOGRAFICO automáticamente
================================================================================

PROPÓSITO:
----------
Crear el trigger que sincroniza automáticamente el campo PUNTO_GEOGRAFICO 
cuando se insertan o actualizan paradas.

NOTA: La columna PUNTO_GEOGRAFICO ya existe en tu tabla (tipo geography)

EJECUTAR:
---------
Simplemente ejecuta este script en SQL Server Management Studio
sobre la base de datos bdMiBus.

================================================================================
*/

USE bdMiBus;
GO

-- Eliminar el trigger si ya existe (para recrearlo limpio)
IF OBJECT_ID('dbo.TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO', 'TR') IS NOT NULL
BEGIN
    DROP TRIGGER dbo.TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO;
    PRINT '? Trigger anterior eliminado';
END
GO

-- Crear el trigger
CREATE TRIGGER TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO
ON TB_PARADA
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- IMPORTANTE: Solo actualizar si LATITUD o LONGITUD cambiaron
    -- Esto evita recursividad infinita
    IF UPDATE(LATITUD) OR UPDATE(LONGITUD) OR NOT EXISTS(SELECT 1 FROM deleted)
    BEGIN
        -- Actualiza PUNTO_GEOGRAFICO usando LATITUD y LONGITUD
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

-- (OPCIONAL) Actualizar registros existentes
UPDATE TB_PARADA
SET PUNTO_GEOGRAFICO = geography::Point(LATITUD, LONGITUD, 4326)
WHERE LATITUD IS NOT NULL 
  AND LONGITUD IS NOT NULL
  AND PUNTO_GEOGRAFICO IS NULL;

DECLARE @Actualizados INT = @@ROWCOUNT;
IF @Actualizados > 0
    PRINT '? ' + CAST(@Actualizados AS VARCHAR) + ' registro(s) existente(s) actualizado(s)';
ELSE
    PRINT '? No hay registros existentes para actualizar';
GO

PRINT '';
PRINT '========================================';
PRINT '? CONFIGURACIÓN COMPLETADA';
PRINT '========================================';
PRINT 'El trigger está activo y funcionando.';
PRINT 'Ahora puedes insertar paradas desde C#';
PRINT 'y PUNTO_GEOGRAFICO se calculará automáticamente.';
GO
