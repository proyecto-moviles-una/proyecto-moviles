-- Reactiva una cuenta desactivada (ESTADO = 2 ? ESTADO = 1) validando el correo y el código enviado.
-- Expira a los 30 minutos igual que los demás códigos del sistema.
CREATE OR ALTER PROCEDURE dbo.SP_REACTIVAR_USUARIO
(
    @CORREO_ELECTRONICO NVARCHAR(100),
    @CODIGO             NVARCHAR(100),
    @IDRETURN           INT OUTPUT,
    @ERRORID            INT OUTPUT,
    @ERRORDESCRIPCION   NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ESTADO               INT;
        DECLARE @CODIGO_GUARDADO      NVARCHAR(100);
        DECLARE @FECHA_CODIGO         DATETIME;

        SELECT @ESTADO          = ESTADO,
               @CODIGO_GUARDADO = NUMERO_VERIFICACION,
               @FECHA_CODIGO    = FECHA_CODIGO_VERIFICACION
        FROM dbo.TB_USUARIO
        WHERE CORREO_ELECTRONICO = @CORREO_ELECTRONICO;

        IF @ESTADO IS NULL
        BEGIN
            SET @IDRETURN        = -1;
            SET @ERRORID         = 14; -- correoNoRegistrado
            SET @ERRORDESCRIPCION = N'CORREO NO REGISTRADO';
            RETURN;
        END

        IF @ESTADO <> 2
        BEGIN
            SET @IDRETURN        = -1;
            SET @ERRORID         = 47; -- usuarioNoDesactivado
            SET @ERRORDESCRIPCION = N'LA CUENTA NO ESTÁ DESACTIVADA';
            RETURN;
        END

        IF @CODIGO_GUARDADO IS NULL OR @FECHA_CODIGO IS NULL
        BEGIN
            SET @IDRETURN        = -1;
            SET @ERRORID         = 12; -- codigoExpirado (sin solicitud previa)
            SET @ERRORDESCRIPCION = N'NO HAY SOLICITUD DE REACTIVACIÓN PENDIENTE';
            RETURN;
        END

        -- Validar expiración (30 minutos)
        IF DATEDIFF(MINUTE, @FECHA_CODIGO, GETDATE()) > 30
        BEGIN
            UPDATE dbo.TB_USUARIO
            SET NUMERO_VERIFICACION       = NULL,
                FECHA_CODIGO_VERIFICACION = NULL
            WHERE CORREO_ELECTRONICO = @CORREO_ELECTRONICO;

            SET @IDRETURN        = -1;
            SET @ERRORID         = 12; -- codigoExpirado
            SET @ERRORDESCRIPCION = N'EL CÓDIGO HA EXPIRADO';
            RETURN;
        END

        IF @CODIGO_GUARDADO <> @CODIGO
        BEGIN
            SET @IDRETURN        = -1;
            SET @ERRORID         = 45; -- codigoVerificacionInvalido
            SET @ERRORDESCRIPCION = N'CÓDIGO INCORRECTO';
            RETURN;
        END

        -- Reactivar cuenta y limpiar código
        UPDATE dbo.TB_USUARIO
        SET ESTADO                    = 1,
            NUMERO_VERIFICACION       = NULL,
            FECHA_CODIGO_VERIFICACION = NULL
        WHERE CORREO_ELECTRONICO = @CORREO_ELECTRONICO
          AND ESTADO             = 2;

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
