-- SP_REENVIAR_ACTIVACION: genera nuevo codigo si el usuario existe y sigue inactivo
CREATE OR ALTER PROCEDURE dbo.SP_REENVIAR_ACTIVACION
(
    @CORREO_ELECTRONICO       NVARCHAR(100),
    @NUEVO_CODIGO             NVARCHAR(MAX),
    @IDRETURN                 INT OUTPUT,
    @ERRORID                  INT OUTPUT,
    @ERRORDESCRIPCION         NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ESTADO INT;
        SELECT @ESTADO = ESTADO FROM dbo.TB_USUARIO WHERE CORREO_ELECTRONICO = @CORREO_ELECTRONICO;

        -- Correo no existe
        IF @ESTADO IS NULL
        BEGIN
            SET @IDRETURN        = -1;
            SET @ERRORID         = 1;
            SET @ERRORDESCRIPCION = N'CORREO NO REGISTRADO';
            RETURN;
        END

        -- Ya esta activo, no necesita reactivacion
        IF @ESTADO = 1
        BEGIN
            SET @IDRETURN        = -1;
            SET @ERRORID         = 2;
            SET @ERRORDESCRIPCION = N'LA CUENTA YA ESTA ACTIVA';
            RETURN;
        END

        -- Esta desactivado (ESTADO=2), no se puede reactivar por este medio
        IF @ESTADO = 2
        BEGIN
            SET @IDRETURN        = -1;
            SET @ERRORID         = 3;
            SET @ERRORDESCRIPCION = N'CUENTA DESACTIVADA';
            RETURN;
        END

        -- ESTADO=0: aun no activado, actualizar codigo y fecha
        UPDATE dbo.TB_USUARIO
        SET NUMERO_VERIFICACION       = @NUEVO_CODIGO,
            FECHA_CODIGO_VERIFICACION = GETUTCDATE()
        WHERE CORREO_ELECTRONICO = @CORREO_ELECTRONICO
          AND ESTADO             = 0;

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
