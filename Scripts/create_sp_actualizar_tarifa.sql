CREATE OR ALTER PROCEDURE dbo.SP_ACTUALIZAR_TARIFA
    @GUID_TARIFA      UNIQUEIDENTIFIER,
    @MONTO            DECIMAL(10,2),
    @FECHA_VIGENCIA   DATETIME,
    @IDRETURN         INT OUTPUT,
    @ERRORID          INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @ID_TARIFA INT;

        SELECT @ID_TARIFA = ID_TARIFA
        FROM dbo.TB_TARIFA
        WHERE GUID_TARIFA = @GUID_TARIFA;

        IF @ID_TARIFA IS NULL
        BEGIN
            SET @IDRETURN = 0;
            SET @ERRORID = 3;
            SET @ERRORDESCRIPCION = 'Tarifa no encontrada';
            RETURN;
        END

        IF @MONTO IS NULL OR @MONTO <= 0
        BEGIN
            SET @IDRETURN = 0;
            SET @ERRORID = 2;
            SET @ERRORDESCRIPCION = 'El monto debe ser mayor a cero';
            RETURN;
        END

        IF @FECHA_VIGENCIA IS NULL
        BEGIN
            SET @IDRETURN = 0;
            SET @ERRORID = 8;
            SET @ERRORDESCRIPCION = 'La fecha de vigencia es obligatoria';
            RETURN;
        END

        UPDATE dbo.TB_TARIFA
        SET MONTO          = @MONTO,
            FECHA_VIGENCIA = @FECHA_VIGENCIA
        WHERE ID_TARIFA = @ID_TARIFA;

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
