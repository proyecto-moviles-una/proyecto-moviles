-- SP_OBTENER_HASH_USUARIO: retorna el hash de contraseña de un usuario por GUID
-- Usado internamente para verificar la contraseña actual antes de cambiarla
CREATE OR ALTER PROCEDURE dbo.SP_OBTENER_HASH_USUARIO
(
    @GUID_USUARIO UNIQUEIDENTIFIER
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        u.GUID_USUARIO,
        u.NOMBRE,
        u.APELLIDOS,
        u.ESTADO,
        u.[PASSWORD] AS HASH_PASSWORD
    FROM dbo.TB_USUARIO u
    WHERE u.GUID_USUARIO = @GUID_USUARIO;
END
