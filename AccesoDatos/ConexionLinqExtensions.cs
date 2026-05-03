// VS NO sobreescribe este archivo al regenerar el DBML.
// Aqui van SPs que no estan en el DBML y extensiones de tipos auto-generados.
using System.Data.Linq;
using System.Reflection;
using System;

namespace AccesoDatos
{
    public partial class ConexionLinqDataContext
    {
        // Versión simplificada de SP_CERRAR_SESION (solo GUID_SESION, retorna @@ROWCOUNT)
        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_CERRAR_SESION")]
        public int SP_CERRAR_SESION(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "GUID_SESION", DbType = "UniqueIdentifier")] System.Nullable<System.Guid> gUID_SESION)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), gUID_SESION);
            return ((int)(result.ReturnValue));
        }

        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_LOGIN")]
        public ISingleResult<SP_LOGINResult> SP_LOGIN(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CORREO_ELECTRONICO", DbType = "NVarChar(100)")] string cORREO_ELECTRONICO)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), cORREO_ELECTRONICO);
            return ((ISingleResult<SP_LOGINResult>)(result.ReturnValue));
        }

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
    }

    public partial class SP_LOGINResult
    {
        private string _HASH_PASSWORD;
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_HASH_PASSWORD", DbType = "NVarChar(MAX)", CanBeNull = true)]
        public string HASH_PASSWORD
        {
            get { return _HASH_PASSWORD; }
            set { _HASH_PASSWORD = value; }
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
}
