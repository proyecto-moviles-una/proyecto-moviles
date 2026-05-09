-- Actualiza SP_CERRAR_SESION para que retorne @@ROWCOUNT como valor de retorno
-- (compatible con la llamada LINQ sin parámetros de salida)
ALTER PROCEDURE dbo.SP_CERRAR_SESION
(
    @GUID_SESION UNIQUEIDENTIFIER
)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.TB_SESION
    SET ESTADO              = 0,
        FECHA_FINAL         = GETUTCDATE(),
        FECHA_ACTUALIZACION = GETUTCDATE()
    WHERE GUID_SESION = @GUID_SESION
      AND ESTADO       = 1;
    RETURN @@ROWCOUNT;
END
