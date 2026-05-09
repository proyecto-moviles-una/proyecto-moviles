-- Genera y guarda un código de reactivación para una cuenta desactivada (ESTADO = 2).
-- Reutiliza NUMERO_VERIFICACION y FECHA_CODIGO_VERIFICACION igual que el flujo de activación inicial.
CREATE OR ALTER PROCEDURE dbo.SP_SOLICITAR_REACTIVACION
(
    @CORREO_ELECTRONICO NVARCHAR(100),
    @CODIGO             NVARCHAR(100),
    @IDRETURN           INT OUTPUT,
    @ERRORID            INT OUTPUT,
    @ERRORDESCRIPCION   NVARCHAR(MAX) OUTPUT,
    @NOMBRE             NVARCHAR(100) OUTPUT,
    @APELLIDOS          NVARCHAR(150) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ESTADO INT;

        SELECT @ESTADO    = ESTADO,
               @NOMBRE    = NOMBRE,
               @APELLIDOS = APELLIDOS
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

        -- Guardar código en los mismos campos que usa activación inicial
        UPDATE dbo.TB_USUARIO
        SET NUMERO_VERIFICACION       = @CODIGO,
            FECHA_CODIGO_VERIFICACION = GETDATE()
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
