-- ============================================================
-- SP_ELIMINAR_USUARIO
-- Elimina físicamente la cuenta del usuario autenticado.
-- También cierra todas sus sesiones activas antes de eliminar.
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.SP_ELIMINAR_USUARIO
(
    @GUID_USUARIO     UNIQUEIDENTIFIER,
    @IDRETURN         INT OUTPUT,
    @ERRORID          INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT OFF;
    BEGIN TRY
        DECLARE @ID_USUARIO BIGINT;
        SELECT @ID_USUARIO = ID_USUARIO FROM dbo.TB_USUARIO WHERE GUID_USUARIO = @GUID_USUARIO;

        IF @ID_USUARIO IS NULL
        BEGIN
            SET @IDRETURN        = -1;
            SET @ERRORID         = 2;
            SET @ERRORDESCRIPCION = N'USUARIO NO ENCONTRADO';
            RETURN;
        END

        -- Cerrar sesiones activas antes de eliminar
        UPDATE dbo.TB_SESION
        SET ESTADO              = 0,
            FECHA_FINAL         = GETUTCDATE(),
            FECHA_ACTUALIZACION = GETUTCDATE()
        WHERE ID_USUARIO = @ID_USUARIO
          AND ESTADO      = 1;

        -- Eliminar favoritos del usuario
        DELETE FROM dbo.TB_FAVORITO WHERE ID_USUARIO = @ID_USUARIO;

        -- Eliminar sesiones
        DELETE FROM dbo.TB_SESION WHERE ID_USUARIO = @ID_USUARIO;

        -- Eliminar el usuario
        DELETE FROM dbo.TB_USUARIO WHERE ID_USUARIO = @ID_USUARIO;

        SET @IDRETURN        = @ID_USUARIO;
        SET @ERRORID         = 0;
        SET @ERRORDESCRIPCION = N'';
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK;
        SET @IDRETURN        = -1;
        SET @ERRORID         = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();
    END CATCH
END
GO

-- Verificar
-- EXEC dbo.SP_ELIMINAR_USUARIO @GUID_USUARIO='...', @IDRETURN=NULL, @ERRORID=NULL, @ERRORDESCRIPCION=NULL
