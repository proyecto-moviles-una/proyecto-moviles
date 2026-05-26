ALTER PROCEDURE dbo.SP_OBTENER_LISTAUSUARIOS
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        u.GUID_USUARIO,
        u.NOMBRE,
        u.APELLIDOS,
        u.CORREO_ELECTRONICO,
        u.FECHA_REGISTRO,
        u.ESTADO,
        CAST(CASE
            WHEN TRY_CONVERT(INT, u.ROL) = 2 OR LOWER(CONVERT(NVARCHAR(50), u.ROL)) IN ('admin', 'administrador') THEN 'admin'
            ELSE 'usuario'
        END AS NVARCHAR(50)) AS ROL
    FROM dbo.TB_USUARIO u;
END
