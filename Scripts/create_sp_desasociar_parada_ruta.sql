CREATE OR ALTER PROCEDURE dbo.SP_DESASOCIAR_PARADA_RUTA
    @GUID_RUTA UNIQUEIDENTIFIER,
    @GUID_PARADA UNIQUEIDENTIFIER,
    @IDRETURN INT OUTPUT,
    @ERRORID INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @ID_RUTA INT;
        DECLARE @ID_PARADA INT;

        SELECT @ID_RUTA = ID_RUTA
        FROM dbo.TB_RUTA
        WHERE GUID_RUTA = @GUID_RUTA;

        IF @ID_RUTA IS NULL
        BEGIN
            SET @IDRETURN = 0;
            SET @ERRORID = 4;
            SET @ERRORDESCRIPCION = 'Ruta no encontrada';
            RETURN;
        END

        SELECT @ID_PARADA = ID_PARADA
        FROM dbo.TB_PARADA
        WHERE GUID_PARADA = @GUID_PARADA;

        IF @ID_PARADA IS NULL
        BEGIN
            SET @IDRETURN = 0;
            SET @ERRORID = 4;
            SET @ERRORDESCRIPCION = 'Parada no encontrada';
            RETURN;
        END

        DELETE FROM dbo.TB_R_RUTA_PARADA
        WHERE ID_RUTA = @ID_RUTA
          AND ID_PARADA = @ID_PARADA;

        IF @@ROWCOUNT = 0
        BEGIN
            SET @IDRETURN = 0;
            SET @ERRORID = 4;
            SET @ERRORDESCRIPCION = 'La parada no estaba asociada a la ruta';
            RETURN;
        END

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
