-- Confirma el cambio de correo validando el código enviado al nuevo correo.
-- Expira a los 30 minutos. Si es correcto, actualiza el correo oficial y limpia los campos pendientes.
CREATE OR ALTER PROCEDURE dbo.SP_CONFIRMAR_CAMBIO_CORREO
(
    @GUID_USUARIO     UNIQUEIDENTIFIER,
    @CODIGO           NVARCHAR(100),
    @IDRETURN         INT OUTPUT,
    @ERRORID          INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @CORREO_PENDIENTE    NVARCHAR(150);
        DECLARE @CODIGO_GUARDADO     NVARCHAR(100);
        DECLARE @FECHA_CODIGO        DATETIME;

        SELECT
            @CORREO_PENDIENTE = CORREO_PENDIENTE,
            @CODIGO_GUARDADO  = CODIGO_CAMBIO_CORREO,
            @FECHA_CODIGO     = FECHA_CODIGO_CORREO
        FROM dbo.TB_USUARIO
        WHERE GUID_USUARIO = @GUID_USUARIO AND ESTADO = 1;

        IF @CORREO_PENDIENTE IS NULL OR @CODIGO_GUARDADO IS NULL
        BEGIN
            SET @IDRETURN        = -1;
            SET @ERRORID         = 12; -- codigoExpirado / sin solicitud pendiente
            SET @ERRORDESCRIPCION = N'NO HAY SOLICITUD DE CAMBIO DE CORREO PENDIENTE';
            RETURN;
        END

        -- Validar que el código no haya expirado (30 minutos)
        IF DATEDIFF(MINUTE, @FECHA_CODIGO, GETDATE()) > 30
        BEGIN
            -- Limpiar campos expirados
            UPDATE dbo.TB_USUARIO
            SET CORREO_PENDIENTE     = NULL,
                CODIGO_CAMBIO_CORREO = NULL,
                FECHA_CODIGO_CORREO  = NULL
            WHERE GUID_USUARIO = @GUID_USUARIO;

            SET @IDRETURN        = -1;
            SET @ERRORID         = 12; -- codigoExpirado
            SET @ERRORDESCRIPCION = N'EL CÓDIGO HA EXPIRADO';
            RETURN;
        END

        -- Validar código
        IF @CODIGO_GUARDADO <> @CODIGO
        BEGIN
            SET @IDRETURN        = -1;
            SET @ERRORID         = 43; -- codigoInvalido
            SET @ERRORDESCRIPCION = N'CÓDIGO DE VERIFICACIÓN INCORRECTO';
            RETURN;
        END

        -- Actualizar correo oficial y limpiar campos pendientes
        UPDATE dbo.TB_USUARIO
        SET CORREO_ELECTRONICO   = @CORREO_PENDIENTE,
            CORREO_PENDIENTE     = NULL,
            CODIGO_CAMBIO_CORREO = NULL,
            FECHA_CODIGO_CORREO  = NULL
        WHERE GUID_USUARIO = @GUID_USUARIO;

        SET @IDRETURN        = @@ROWCOUNT;
        SET @ERRORID         = 0;
        SET @ERRORDESCRIPCION = N'';
    END TRY
    BEGIN CATCH
        SET @IDRETURN        = -1;
        SET @ERRORID         = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();
        INSERT INTO dbo.TB_ERROR_EN_BASE_DATOS(SEVERIDAD, STORED_PROCEDURE, NUMERO, DESCRIPCION, LINEA)
        SELECT ERROR_SEVERITY(), ERROR_PROCEDURE(), ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE();
    END CATCH
END
