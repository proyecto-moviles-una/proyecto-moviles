using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;

namespace AccesoDatos
{
    public partial class ConexionLinqDataContext
    {
        [Function(Name = "dbo.SP_DESASOCIAR_PARADA_RUTA")]
        public int SP_DESASOCIAR_PARADA_RUTA(
            [Parameter(Name = "GUID_RUTA", DbType = "UniqueIdentifier")] Guid? guidRuta,
            [Parameter(Name = "GUID_PARADA", DbType = "UniqueIdentifier")] Guid? guidParada,
            [Parameter(Name = "IDRETURN", DbType = "Int")] ref int? idReturn,
            [Parameter(Name = "ERRORID", DbType = "Int")] ref int? errorId,
            [Parameter(Name = "ERRORDESCRIPCION", DbType = "NVarChar(MAX)")] ref string errorDescripcion)
        {
            IExecuteResult result = ExecuteMethodCall(
                this,
                (MethodInfo)MethodInfo.GetCurrentMethod(),
                guidRuta,
                guidParada,
                idReturn,
                errorId,
                errorDescripcion);

            idReturn = (int?)result.GetParameterValue(2);
            errorId = (int?)result.GetParameterValue(3);
            errorDescripcion = (string)result.GetParameterValue(4);

            return (int)result.ReturnValue;
        }
    }
}
