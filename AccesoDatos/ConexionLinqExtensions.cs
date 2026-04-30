// Extensiones del DataContext — VS NO sobreescribe este archivo al regenerar el DBML.
// VS NO sobreescribe este archivo al regenerar el DBML.
// Aqui van SPs que no estan en el DBML y extensiones de tipos auto-generados.
using System.Data.Linq;
using System.Reflection;
using System;

namespace AccesoDatos
{
    public partial class ConexionLinqDataContext
    {
        // SP_LOGIN solo por correo (sin PASSWORD) — el SP fue modificado
        // para retornar HASH_PASSWORD y que BCrypt compare en C#.
        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_LOGIN")]
        public ISingleResult<SP_LOGINResult> SP_LOGIN(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "CORREO_ELECTRONICO", DbType = "NVarChar(100)")] string cORREO_ELECTRONICO)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), cORREO_ELECTRONICO);
            return ((ISingleResult<SP_LOGINResult>)(result.ReturnValue));
        }

        // SP_CERRAR_SESION fue modificado para retornar outputs (filas, errorId, errorDesc)
        // El designer tiene la version sin outputs — se sobreescribe aqui.
        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.SP_CERRAR_SESION")]
        public int SP_CERRAR_SESION(
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "GUID_SESION",        DbType = "UniqueIdentifier")] System.Nullable<System.Guid> gUID_SESION,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "FILASACTUALIZADAS",  DbType = "Int")]              ref System.Nullable<int>      fILASACTUALIZADAS,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORID",            DbType = "Int")]              ref System.Nullable<int>      eRRORID,
            [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ERRORDESCRIPCION",   DbType = "NVarChar(MAX)")]    ref string                    eRRORDESCRIPCION)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                gUID_SESION, fILASACTUALIZADAS, eRRORID, eRRORDESCRIPCION);
            fILASACTUALIZADAS = ((System.Nullable<int>)(result.GetParameterValue(1)));
            eRRORID           = ((System.Nullable<int>)(result.GetParameterValue(2)));
            eRRORDESCRIPCION  = ((string)(result.GetParameterValue(3)));
            return ((int)(result.ReturnValue));
        }
    }

    // Agrega HASH_PASSWORD al resultado de SP_LOGIN para verificacion BCrypt
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

    // Agrega ESTADO al resultado de SP_OBTENER_LISTAUSUARIOS
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





