-- SP_CERRAR_SESION: falla si la sesion ya fue cerrada (no permite doble logout)
ALTER PROCEDURE dbo.SP_CERRAR_SESION
(
    @GUID_SESION      UNIQUEIDENTIFIER,
    @FILASACTUALIZADAS INT OUTPUT,
    @ERRORID          INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Solo cerrar si la sesion existe y esta activa
        UPDATE dbo.TB_SESION
        SET ESTADO              = 0,
            FECHA_FINAL         = GETUTCDATE(),
            FECHA_ACTUALIZACION = GETUTCDATE()
        WHERE GUID_SESION = @GUID_SESION
          AND ESTADO       = 1;

        SET @FILASACTUALIZADAS = @@ROWCOUNT;

        IF @FILASACTUALIZADAS = 0
        BEGIN
            SET @ERRORID         = 1;
            SET @ERRORDESCRIPCION = N'SESION NO ACTIVA O NO EXISTE';
        END
        ELSE
        BEGIN
            SET @ERRORID         = 0;
            SET @ERRORDESCRIPCION = N'';
        END
    END TRY
    BEGIN CATCH
        SET @FILASACTUALIZADAS = 0;
        SET @ERRORID           = ERROR_NUMBER();
        SET @ERRORDESCRIPCION  = ERROR_MESSAGE();
    END CATCH
END
