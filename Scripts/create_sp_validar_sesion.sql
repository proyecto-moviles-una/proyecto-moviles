-- SP_VALIDAR_SESION: retorna datos de la sesion si existe y esta activa
-- El DBML ya lo tiene con 1 parametro, el C# valida el GUID_USUARIO
CREATE OR ALTER PROCEDURE dbo.SP_VALIDAR_SESION
(
    @GUID_SESION UNIQUEIDENTIFIER
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.GUID_SESION,
        u.GUID_USUARIO,
        u.NOMBRE,
        u.APELLIDOS,
        s.ESTADO
    FROM dbo.TB_SESION s
    INNER JOIN dbo.TB_USUARIO u ON u.ID_USUARIO = s.ID_USUARIO
    WHERE s.GUID_SESION = @GUID_SESION;
END


