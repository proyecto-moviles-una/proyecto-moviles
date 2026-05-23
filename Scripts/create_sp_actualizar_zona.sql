CREATE OR ALTER PROCEDURE dbo.SP_ACTUALIZAR_ZONA
    @GUID_ZONA       UNIQUEIDENTIFIER,
    @NOMBRE          NVARCHAR(100),
    @DESCRIPCION     NVARCHAR(MAX),
    @IDRETURN        INT OUTPUT,
    @ERRORID         INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @ID_ZONA INT;

        SELECT @ID_ZONA = ID_ZONA
        FROM dbo.TB_ZONA
        WHERE GUID_ZONA = @GUID_ZONA;

        IF @ID_ZONA IS NULL
        BEGIN
            SET @IDRETURN = 0;
            SET @ERRORID = 2;
            SET @ERRORDESCRIPCION = 'Zona no encontrada';
            RETURN;
        END

        IF @NOMBRE IS NULL OR LTRIM(RTRIM(@NOMBRE)) = ''
        BEGIN
            SET @IDRETURN = 0;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'El nombre es obligatorio';
            RETURN;
        END

        UPDATE dbo.TB_ZONA
        SET NOMBRE      = @NOMBRE,
            DESCRIPCION = @DESCRIPCION
        WHERE ID_ZONA = @ID_ZONA;

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
