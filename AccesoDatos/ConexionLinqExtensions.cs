// VS NO sobreescribe este archivo al regenerar el DBML.
// Aqui van SPs que no estan en el DBML y extensiones de tipos auto-generados.
using System.Data.Linq;
using System.Reflection;
using System;

namespace AccesoDatos
{
    public partial class ConexionLinqDataContext
    {
        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_OBTENER_HASH_USUARIO")]
        public ISingleResult<SP_LOGINResult> SP_OBTENER_HASH_USUARIO(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "GUID_USUARIO", DbType = "UniqueIdentifier")] System.Nullable<System.Guid> gUID_USUARIO)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), gUID_USUARIO);
            return ((ISingleResult<SP_LOGINResult>)(result.ReturnValue));
        }

        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_CAMBIAR_PASSWORD")]
        public int SP_CAMBIAR_PASSWORD(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "GUID_USUARIO",     DbType = "UniqueIdentifier")] System.Nullable<System.Guid> gUID_USUARIO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "NUEVO_PASSWORD",   DbType = "NVarChar(MAX)")]    string nUEVO_PASSWORD,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "IDRETURN",         DbType = "Int")]              ref System.Nullable<int> iDRETURN,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORID",         DbType = "Int")]              ref System.Nullable<int> eRRORID,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORDESCRIPCION", DbType = "NVarChar(MAX)")]    ref string eRRORDESCRIPCION)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                gUID_USUARIO, nUEVO_PASSWORD, iDRETURN, eRRORID, eRRORDESCRIPCION);
            iDRETURN         = ((System.Nullable<int>)(result.GetParameterValue(2)));
            eRRORID          = ((System.Nullable<int>)(result.GetParameterValue(3)));
            eRRORDESCRIPCION = ((string)(result.GetParameterValue(4)));
            return ((int)(result.ReturnValue));
        }

        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_REENVIAR_ACTIVACION")]
        public int SP_REENVIAR_ACTIVACION(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CORREO_ELECTRONICO", DbType = "NVarChar(100)")] string cORREO_ELECTRONICO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "NUEVO_CODIGO",       DbType = "NVarChar(MAX)")] string nUEVO_CODIGO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "IDRETURN",           DbType = "Int")]           ref System.Nullable<int> iDRETURN,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORID",            DbType = "Int")]           ref System.Nullable<int> eRRORID,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORDESCRIPCION",   DbType = "NVarChar(MAX)")] ref string eRRORDESCRIPCION)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                cORREO_ELECTRONICO, nUEVO_CODIGO, iDRETURN, eRRORID, eRRORDESCRIPCION);
            iDRETURN         = ((System.Nullable<int>)(result.GetParameterValue(2)));
            eRRORID          = ((System.Nullable<int>)(result.GetParameterValue(3)));
            eRRORDESCRIPCION = ((string)(result.GetParameterValue(4)));
            return ((int)(result.ReturnValue));
        }

        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_SOLICITAR_CAMBIO_CORREO")]
        public int SP_SOLICITAR_CAMBIO_CORREO(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "GUID_USUARIO",     DbType = "UniqueIdentifier")] System.Nullable<System.Guid> gUID_USUARIO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CORREO_NUEVO",     DbType = "NVarChar(150)")]    string cORREO_NUEVO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CODIGO",           DbType = "NVarChar(100)")]    string cODIGO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "IDRETURN",         DbType = "Int")]              ref System.Nullable<int> iDRETURN,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORID",          DbType = "Int")]              ref System.Nullable<int> eRRORID,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORDESCRIPCION", DbType = "NVarChar(MAX)")]    ref string eRRORDESCRIPCION)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                gUID_USUARIO, cORREO_NUEVO, cODIGO, iDRETURN, eRRORID, eRRORDESCRIPCION);
            iDRETURN         = ((System.Nullable<int>)(result.GetParameterValue(3)));
            eRRORID          = ((System.Nullable<int>)(result.GetParameterValue(4)));
            eRRORDESCRIPCION = ((string)(result.GetParameterValue(5)));
            return ((int)(result.ReturnValue));
        }

        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_CONFIRMAR_CAMBIO_CORREO")]
        public int SP_CONFIRMAR_CAMBIO_CORREO(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "GUID_USUARIO",     DbType = "UniqueIdentifier")] System.Nullable<System.Guid> gUID_USUARIO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CODIGO",           DbType = "NVarChar(100)")]    string cODIGO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "IDRETURN",         DbType = "Int")]              ref System.Nullable<int> iDRETURN,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORID",          DbType = "Int")]              ref System.Nullable<int> eRRORID,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORDESCRIPCION", DbType = "NVarChar(MAX)")]    ref string eRRORDESCRIPCION)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                gUID_USUARIO, cODIGO, iDRETURN, eRRORID, eRRORDESCRIPCION);
            iDRETURN         = ((System.Nullable<int>)(result.GetParameterValue(2)));
            eRRORID          = ((System.Nullable<int>)(result.GetParameterValue(3)));
            eRRORDESCRIPCION = ((string)(result.GetParameterValue(4)));
            return ((int)(result.ReturnValue));
        }

        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_SOLICITAR_REACTIVACION")]
        public int SP_SOLICITAR_REACTIVACION(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CORREO_ELECTRONICO", DbType = "NVarChar(100)")] string cORREO_ELECTRONICO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CODIGO",             DbType = "NVarChar(100)")] string cODIGO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "IDRETURN",           DbType = "Int")]           ref System.Nullable<int> iDRETURN,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORID",            DbType = "Int")]           ref System.Nullable<int> eRRORID,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORDESCRIPCION",   DbType = "NVarChar(MAX)")] ref string eRRORDESCRIPCION,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "NOMBRE",             DbType = "NVarChar(100)")] ref string nOMBRE,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "APELLIDOS",          DbType = "NVarChar(150)")] ref string aPELLIDOS)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                cORREO_ELECTRONICO, cODIGO, iDRETURN, eRRORID, eRRORDESCRIPCION, nOMBRE, aPELLIDOS);
            iDRETURN         = ((System.Nullable<int>)(result.GetParameterValue(2)));
            eRRORID          = ((System.Nullable<int>)(result.GetParameterValue(3)));
            eRRORDESCRIPCION = ((string)(result.GetParameterValue(4)));
            nOMBRE           = ((string)(result.GetParameterValue(5)));
            aPELLIDOS        = ((string)(result.GetParameterValue(6)));
            return ((int)(result.ReturnValue));
        }

        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_REACTIVAR_USUARIO")]
        public int SP_REACTIVAR_USUARIO(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CORREO_ELECTRONICO", DbType = "NVarChar(100)")] string cORREO_ELECTRONICO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CODIGO",             DbType = "NVarChar(100)")] string cODIGO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "IDRETURN",           DbType = "Int")]           ref System.Nullable<int> iDRETURN,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORID",            DbType = "Int")]           ref System.Nullable<int> eRRORID,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORDESCRIPCION",   DbType = "NVarChar(MAX)")] ref string eRRORDESCRIPCION)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                cORREO_ELECTRONICO, cODIGO, iDRETURN, eRRORID, eRRORDESCRIPCION);
            iDRETURN         = ((System.Nullable<int>)(result.GetParameterValue(2)));
            eRRORID          = ((System.Nullable<int>)(result.GetParameterValue(3)));
            eRRORDESCRIPCION = ((string)(result.GetParameterValue(4)));
            return ((int)(result.ReturnValue));
        }

        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_ACTUALIZAR_TOKEN_FCM")]
        public int SP_ACTUALIZAR_TOKEN_FCM(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "GUID_USUARIO",     DbType = "UniqueIdentifier")] System.Nullable<System.Guid> gUID_USUARIO,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "TOKEN_FCM",        DbType = "NVarChar(500)")]     string tOKEN_FCM,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "FILASACTUALIZADAS", DbType = "Int")]             ref System.Nullable<int> fILASACTUALIZADAS,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "IDRETURN",         DbType = "Int")]              ref System.Nullable<int> iDRETURN,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORID",          DbType = "Int")]              ref System.Nullable<int> eRRORID,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORDESCRIPCION", DbType = "NVarChar(MAX)")]    ref string eRRORDESCRIPCION)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                gUID_USUARIO, tOKEN_FCM, fILASACTUALIZADAS, iDRETURN, eRRORID, eRRORDESCRIPCION);
            fILASACTUALIZADAS = ((System.Nullable<int>)(result.GetParameterValue(2)));
            iDRETURN          = ((System.Nullable<int>)(result.GetParameterValue(3)));
            eRRORID           = ((System.Nullable<int>)(result.GetParameterValue(4)));
            eRRORDESCRIPCION  = ((string)(result.GetParameterValue(5)));
            return ((int)(result.ReturnValue));
        }

        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_OBTENER_TOKENS_FCM_POR_RUTA")]
        public ISingleResult<SP_OBTENER_TOKENS_FCM_POR_RUTAResult> SP_OBTENER_TOKENS_FCM_POR_RUTA(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "GUID_RUTA", DbType = "UniqueIdentifier")] System.Nullable<System.Guid> gUID_RUTA)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), gUID_RUTA);
            return ((ISingleResult<SP_OBTENER_TOKENS_FCM_POR_RUTAResult>)(result.ReturnValue));
        }
    }

    public partial class SP_OBTENER_TOKENS_FCM_POR_RUTAResult
    {
        private string _TOKEN_FCM;
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_TOKEN_FCM", DbType = "NVarChar(500)", CanBeNull = true)]
        public string TOKEN_FCM
        {
            get { return _TOKEN_FCM; }
            set { _TOKEN_FCM = value; }
        }
    }

    public partial class SP_LOGINResult
    {
        private string _ROL;
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ROL", DbType = "NVarChar(50)", CanBeNull = true)]
        public string ROL
        {
            get { return _ROL; }
            set { _ROL = value; }
        }
    }

    public partial class SP_OBTENER_LISTAUSUARIOSResult
    {
        private System.Nullable<int> _ESTADO;
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ESTADO", DbType = "Int", CanBeNull = true)]
        public System.Nullable<int> ESTADO
        {
            get { return _ESTADO; }
            set { _ESTADO = value; }
        }
    }

    // Agrega ORIGEN y DESTINO al resultado de SP_OBTENER_FAVORITOS_POR_USUARIO
    // (columnas nuevas no presentes cuando se generó el DBML)
    public partial class SP_OBTENER_FAVORITOS_POR_USUARIOResult
    {
        private string _ORIGEN;
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ORIGEN", DbType = "NVarChar(200)", CanBeNull = true)]
        public string ORIGEN
        {
            get { return _ORIGEN; }
            set { _ORIGEN = value; }
        }

        private string _DESTINO;
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_DESTINO", DbType = "NVarChar(200)", CanBeNull = true)]
        public string DESTINO
        {
            get { return _DESTINO; }
            set { _DESTINO = value; }
        }
    }
}
