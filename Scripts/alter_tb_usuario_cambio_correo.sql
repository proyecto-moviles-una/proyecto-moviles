-- Agrega columnas para el flujo de cambio de correo pendiente
-- Correr una sola vez en BD
ALTER TABLE dbo.TB_USUARIO
    ADD CORREO_PENDIENTE       NVARCHAR(150) NULL,
        CODIGO_CAMBIO_CORREO   NVARCHAR(100) NULL,
        FECHA_CODIGO_CORREO    DATETIME      NULL;
