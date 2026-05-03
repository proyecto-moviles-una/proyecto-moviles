using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Empresa
{
    public class LogEmpresa
    {
        // =====================================================================
        // INSERTAR EMPRESA
        // =====================================================================
        public ResInsertarEmpresa insertar(ReqInsertarEmpresa req)
        {
            ResInsertarEmpresa res = new ResInsertarEmpresa();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(req.nombre))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.nombreEmpresaFaltante));
                    return res;
                }

                System.Nullable<System.Guid> guidEmpresa = null;
                System.Nullable<int>         idReturn    = null;
                System.Nullable<int>         errorIdBD   = null;
                string                       errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_INGRESAR_EMPRESA(
                        req.nombre,
                        req.telefono,
                        req.correo,
                        ref guidEmpresa,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (guidEmpresa == null || guidEmpresa == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorInsertandoEmpresa));
                    errorId   = (int)enumErrores.errorInsertandoEmpresa;
                    errorDesc = errorDescBD ?? enumErrores.errorInsertandoEmpresa.ToString();
                    return res;
                }

                res.resultado   = true;
                res.guidEmpresa = guidEmpresa;
                res.error       = null;
                tipoBitacora    = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId   = (int)enumErrores.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(null, tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // =====================================================================
        // OBTENER EMPRESAS
        // =====================================================================
        public ResObtenerEmpresas obtener(ReqObtenerEmpresas req)
        {
            ResObtenerEmpresas res = new ResObtenerEmpresas();
            res.resultado = false;
            res.error     = new List<Error>();
            res.empresas  = new List<Core.Entidades.Empresa>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                List<SP_OBTENER_EMPRESASResult> lista;
                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    lista = linq.SP_OBTENER_EMPRESAS().ToList();
                }

                res.empresas  = factoriaEmpresas(lista);
                res.resultado = true;
                res.error     = null;
                tipoBitacora  = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId   = (int)enumErrores.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(null, tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // =====================================================================
        // FACTORÍA
        // =====================================================================
        private List<Core.Entidades.Empresa> factoriaEmpresas(List<SP_OBTENER_EMPRESASResult> lista)
        {
            List<Core.Entidades.Empresa> resultado = new List<Core.Entidades.Empresa>();
            foreach (var sp in lista)
            {
                resultado.Add(new Core.Entidades.Empresa
                {
                    guidEmpresa   = sp.GUID_EMPRESA,
                    nombre        = sp.NOMBRE,
                    telefono      = sp.TELEFONO,
                    correo        = sp.CORREO,
                    estado        = sp.ESTADO,
                    fechaRegistro = sp.FECHA_REGISTRO
                });
            }
            return resultado;
        }

        // =====================================================================
        // BITÁCORA
        // =====================================================================
        private void bitacorear(Guid? guidUsuario, enumBitacora tipo, int errorId,
                                string errorDesc, object req, object res)
        {
            try
            {
                ReqBitacorear reqBit = new ReqBitacorear();
                reqBit.bitacora = new Bitacora
                {
                    guidUsuario = guidUsuario,
                    clase       = GetType().Name,
                    metodo      = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name,
                    tipo        = tipo,
                    errorId     = errorId,
                    descripcion = errorDesc,
                    request     = JsonConvert.SerializeObject(req),
                    response    = JsonConvert.SerializeObject(res)
                };
                Utilitarios.Utilitarios.bitacorear(reqBit);
            }
            catch { }
        }
    }
}
