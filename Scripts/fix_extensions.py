import sys
src = '''// VS NO sobreescribe este archivo al regenerar el DBML.
// Aqui van SPs que no estan en el DBML y extensiones de tipos auto-generados.
using System.Data.Linq;
using System.Reflection;
using System;

namespace AccesoDatos
{
    public partial class ConexionLinqDataContext
    {
        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_LOGIN")]
        public ISingleResult<SP_LOGINResult> SP_LOGIN(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CORREO_ELECTRONICO", DbType = "NVarChar(100)")] string cORREO_ELECTRONICO)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), cORREO_ELECTRONICO);
            return ((ISingleResult<SP_LOGINResult>)(result.ReturnValue));
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
'''
with open(r'C:\Proyecto Moviles\AccesoDatos\ConexionLinqExtensions.cs', 'w', encoding='utf-8-sig') as f:
    f.write(src)
print('OK')
