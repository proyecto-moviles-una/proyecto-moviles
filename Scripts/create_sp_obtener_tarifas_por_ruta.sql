CREATE OR ALTER PROCEDURE dbo.SP_OBTENER_TARIFAS_POR_RUTA
    @GUID_RUTA UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @ID_RUTA INT;

        SELECT @ID_RUTA = ID_RUTA
        FROM dbo.TB_RUTA
        WHERE GUID_RUTA = @GUID_RUTA;

        IF @ID_RUTA IS NULL
        BEGIN
            SELECT CAST(NULL AS UNIQUEIDENTIFIER) AS GUID_TARIFA,
                   CAST(NULL AS UNIQUEIDENTIFIER) AS GUID_RUTA,
                   CAST(0 AS DECIMAL(10,2))       AS MONTO,
                   CAST(NULL AS DATETIME)          AS FECHA_VIGENCIA,
                   CAST(0 AS BIT)                 AS ESTADO
            WHERE 1 = 0;
            RETURN;
        END

        SELECT t.GUID_TARIFA,
               r.GUID_RUTA,
               t.MONTO,
               t.FECHA_VIGENCIA,
               t.ESTADO
        FROM dbo.TB_TARIFA t
        INNER JOIN dbo.TB_RUTA r ON r.ID_RUTA = t.ID_RUTA
        WHERE t.ID_RUTA = @ID_RUTA
          AND t.ESTADO = 1;
    END TRY
    BEGIN CATCH
        SELECT CAST(NULL AS UNIQUEIDENTIFIER) AS GUID_TARIFA,
               CAST(NULL AS UNIQUEIDENTIFIER) AS GUID_RUTA,
               CAST(0 AS DECIMAL(10,2))       AS MONTO,
               CAST(NULL AS DATETIME)          AS FECHA_VIGENCIA,
               CAST(0 AS BIT)                 AS ESTADO
        WHERE 1 = 0;
    END CATCH
END
GO
