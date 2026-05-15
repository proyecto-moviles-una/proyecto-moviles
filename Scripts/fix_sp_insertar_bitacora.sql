-- ============================================================
-- FIX: SP_INSERTAR_BITACORA
-- Problema: el SP fue modificado para recibir @DISPOSITIVO como
--           primer parámetro, pero el código C# (LINQ) lo llama
--           con 7 parámetros: CLASE, METODO, TIPO, CODIGO_ERROR,
--           DESCRIPCION, REQUEST, RESPONSE.
--           Esa diferencia hace que el SP falle silenciosamente
--           (el catch vacío en Utilitarios.bitacorear lo traga).
--
-- Solución: re-crear el SP con la firma exacta que usa el C#.
--           ID_USUARIO puede quedar NULL, no hay problema.
-- ============================================================

-- Paso 1: eliminar columna DISPOSITIVO si se agregó antes (opcional, 
--         déjala si ya la tenés y querés conservarla para otro uso)
-- IF EXISTS (
--     SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
--     WHERE TABLE_NAME = 'TB_BITACORA' AND COLUMN_NAME = 'DISPOSITIVO'
-- )
-- BEGIN
--     ALTER TABLE dbo.TB_BITACORA DROP COLUMN DISPOSITIVO;
-- END
-- GO

-- Paso 2: crear la tabla TB_BITACORA si no existe
IF NOT EXISTS (
    SELECT 1 FROM sys.tables
    WHERE name = 'TB_BITACORA' AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE dbo.TB_BITACORA (
        ID             INT              IDENTITY(1,1) NOT NULL PRIMARY KEY,
        GUID_BITACORA  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        CLASE          NVARCHAR(100)    NULL,
        METODO         NVARCHAR(100)    NULL,
        TIPO           SMALLINT         NULL,
        CODIGO_ERROR   INT              NULL,
        DESCRIPCION    NVARCHAR(MAX)    NULL,
        REQUEST        NVARCHAR(MAX)    NULL,
        RESPONSE       NVARCHAR(MAX)    NULL,
        DISPOSITIVO    NVARCHAR(200)    NULL,
        FECHA_REGISTRO DATETIME         NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Tabla TB_BITACORA creada correctamente.';
END
ELSE
BEGIN
    PRINT 'Tabla TB_BITACORA ya existe.';
    -- Agregar DISPOSITIVO si no existe
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='TB_BITACORA' AND COLUMN_NAME='DISPOSITIVO')
        ALTER TABLE dbo.TB_BITACORA ADD DISPOSITIVO NVARCHAR(200) NULL;
    -- Quitar ID_USUARIO si existe
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='TB_BITACORA' AND COLUMN_NAME='ID_USUARIO')
        ALTER TABLE dbo.TB_BITACORA DROP COLUMN ID_USUARIO;
END
GO

-- Paso 3: crear o reemplazar el SP
CREATE OR ALTER PROCEDURE dbo.SP_INSERTAR_BITACORA
(
    @CLASE        NVARCHAR(100),
    @METODO       NVARCHAR(100),
    @TIPO         SMALLINT,
    @CODIGO_ERROR INT,
    @DESCRIPCION  NVARCHAR(MAX),
    @REQUEST      NVARCHAR(MAX),
    @RESPONSE     NVARCHAR(MAX),
    @DISPOSITIVO  NVARCHAR(200)
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.TB_BITACORA
            (GUID_BITACORA, CLASE, METODO, TIPO, CODIGO_ERROR, DESCRIPCION, REQUEST, RESPONSE, DISPOSITIVO, FECHA_REGISTRO)
        VALUES
            (NEWID(), @CLASE, @METODO, @TIPO, @CODIGO_ERROR, @DESCRIPCION, @REQUEST, @RESPONSE, @DISPOSITIVO, GETDATE());
    END TRY
    BEGIN CATCH
        -- La bitácora no debe bloquear el flujo principal
    END CATCH
END
GO

-- Paso 4: verificar
SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'TB_BITACORA' ORDER BY ORDINAL_POSITION;
GO
