-- Guarda el correo pendiente y el código de verificación para el cambio de correo.
-- La validación del password actual se hace en C# antes de llamar este SP.
CREATE OR ALTER PROCEDURE dbo.SP_SOLICITAR_CAMBIO_CORREO
(
    @GUID_USUARIO        UNIQUEIDENTIFIER,
    @CORREO_NUEVO        NVARCHAR(150),
    @CODIGO              NVARCHAR(100),
    @IDRETURN            INT OUTPUT,
    @ERRORID             INT OUTPUT,
    @ERRORDESCRIPCION    NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Verificar que el nuevo correo no esté en uso por otro usuario activo
        IF EXISTS (
            SELECT 1 FROM dbo.TB_USUARIO
            WHERE CORREO_ELECTRONICO = @CORREO_NUEVO
              AND GUID_USUARIO <> @GUID_USUARIO
        )
        BEGIN
            SET @IDRETURN        = -1;
            SET @ERRORID         = 1;  -- correoYaRegistrado
            SET @ERRORDESCRIPCION = N'EL CORREO NUEVO YA ESTÁ EN USO';
            RETURN;
        END

        -- Verificar que el usuario existe y está activo
        IF NOT EXISTS (
            SELECT 1 FROM dbo.TB_USUARIO
            WHERE GUID_USUARIO = @GUID_USUARIO AND ESTADO = 1
        )
        BEGIN
            SET @IDRETURN        = -1;
            SET @ERRORID         = 6;  -- guidDeUsuarioFaltante
            SET @ERRORDESCRIPCION = N'USUARIO NO ENCONTRADO O INACTIVO';
            RETURN;
        END

        -- Guardar correo pendiente y código (expira en 30 min)
        UPDATE dbo.TB_USUARIO
        SET CORREO_PENDIENTE     = @CORREO_NUEVO,
            CODIGO_CAMBIO_CORREO = @CODIGO,
            FECHA_CODIGO_CORREO  = GETDATE()
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
