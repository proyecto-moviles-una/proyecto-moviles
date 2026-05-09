-- Agrega la columna GUID_ERROR_BD a TB_ERROR_EN_BASE_DATOS si aún no existe.
-- Ejecutar solo una vez sobre la BD que ya tiene la tabla creada sin el GUID.
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA  = 'dbo'
      AND TABLE_NAME    = 'TB_ERROR_EN_BASE_DATOS'
      AND COLUMN_NAME   = 'GUID_ERROR_BD'
)
BEGIN
    ALTER TABLE dbo.TB_ERROR_EN_BASE_DATOS
        ADD GUID_ERROR_BD UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();

    PRINT 'Columna GUID_ERROR_BD agregada correctamente.';
END
ELSE
BEGIN
    PRINT 'La columna GUID_ERROR_BD ya existe. No se hizo nada.';
END
