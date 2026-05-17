CREATE OR ALTER PROCEDURE dbo.SP_ACTUALIZAR_EMPRESA
    @GUID_EMPRESA     UNIQUEIDENTIFIER,
    @NOMBRE           NVARCHAR(150),
    @TELEFONO         NVARCHAR(20),
    @CORREO           NVARCHAR(100),
    @IDRETURN         INT OUTPUT,
    @ERRORID          INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @ID_EMPRESA INT;

        SELECT @ID_EMPRESA = ID_EMPRESA
        FROM dbo.TB_EMPRESA
        WHERE GUID_EMPRESA = @GUID_EMPRESA;

        IF @ID_EMPRESA IS NULL
        BEGIN
            SET @IDRETURN = 0;
            SET @ERRORID = 2;
            SET @ERRORDESCRIPCION = 'Empresa no encontrada';
            RETURN;
        END

        IF @NOMBRE IS NULL OR LTRIM(RTRIM(@NOMBRE)) = ''
        BEGIN
            SET @IDRETURN = 0;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'El nombre es obligatorio';
            RETURN;
        END

        UPDATE dbo.TB_EMPRESA
        SET NOMBRE    = @NOMBRE,
            TELEFONO  = @TELEFONO,
            CORREO    = @CORREO
        WHERE ID_EMPRESA = @ID_EMPRESA;

        SET @IDRETURN = 1;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = NULL;
    END TRY
    BEGIN CATCH
        SET @IDRETURN = 0;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();
    END CATCH
END
GO
