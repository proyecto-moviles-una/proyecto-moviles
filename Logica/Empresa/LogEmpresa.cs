using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum;
using Core.Enum.Empresa;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Empresa
{
    public class LogEmpresa
    {
        public ResCrearEmpresa Crear(ReqCrearEmpresa req)
        {
            var res = new ResCrearEmpresa { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(req.Nombre))
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.nombreFaltante, Mensaje = "El nombre es obligatorio" });
                    errorId = (int)EnumErroresEmpresa.nombreFaltante;
                    errorDesc = "El nombre es obligatorio";
                    return res;
                }

                System.Nullable<System.Guid> guidEmpresa = null;
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_INGRESAR_EMPRESA(req.Nombre, req.Telefono, req.Correo, ref guidEmpresa, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (guidEmpresa == null || guidEmpresa == Guid.Empty)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.errorCreandoEmpresa, Mensaje = errorDescBD ?? "Error al crear la empresa" });
                    errorId = (int)EnumErroresEmpresa.errorCreandoEmpresa;
                    errorDesc = errorDescBD ?? "Error al crear la empresa";
                    return res;
                }

                res.Empresa = new Core.Entidades.Empresa { Guid = guidEmpresa.Value, Nombre = req.Nombre, Telefono = req.Telefono, Correo = req.Correo, Estado = true };
                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.errorCreandoEmpresa, Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "") });
                errorId = (int)EnumErroresEmpresa.errorCreandoEmpresa;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        public ResListarEmpresas Listar()
        {
            var res = new ResListarEmpresas { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    res.Empresas = db.SP_OBTENER_EMPRESAS().Select(x => new Core.Entidades.Empresa
                    {
                        Guid = x.GUID_EMPRESA,
                        Nombre = x.NOMBRE,
                        Telefono = x.TELEFONO,
                        Correo = x.CORREO,
                        Estado = x.ESTADO
                    }).ToList();
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.errorCreandoEmpresa, Mensaje = ex.Message });
                errorId = (int)EnumErroresEmpresa.errorCreandoEmpresa;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, null, res);
            }

            return res;
        }

        public ResCrearEmpresa ObtenerPorGuid(Guid guid)
        {
            var res = new ResCrearEmpresa { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    var e = db.SP_OBTENER_EMPRESAS().FirstOrDefault(x => x.GUID_EMPRESA == guid);

                    if (e == null)
                    {
                        res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.empresaNoEncontrada, Mensaje = "Empresa no encontrada" });
                        errorId = (int)EnumErroresEmpresa.empresaNoEncontrada;
                        errorDesc = "Empresa no encontrada";
                        return res;
                    }

                    res.Empresa = new Core.Entidades.Empresa { Guid = e.GUID_EMPRESA, Nombre = e.NOMBRE, Telefono = e.TELEFONO, Correo = e.CORREO, Estado = e.ESTADO };
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.errorCreandoEmpresa, Mensaje = ex.Message });
                errorId = (int)EnumErroresEmpresa.errorCreandoEmpresa;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, guid, res);
            }

            return res;
        }

        public ResCrearEmpresa Editar(Guid guid, ReqCrearEmpresa req)
        {
            var res = new ResCrearEmpresa { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(req.Nombre))
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.nombreFaltante, Mensaje = "El nombre es obligatorio" });
                    errorId = (int)EnumErroresEmpresa.nombreFaltante;
                    errorDesc = "El nombre es obligatorio";
                    return res;
                }

                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_ACTUALIZAR_EMPRESA(guid, req.Nombre, req.Telefono, req.Correo, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (errorIdBD.HasValue && errorIdBD.Value != 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.errorEditandoEmpresa, Mensaje = errorDescBD ?? "Error al actualizar la empresa" });
                    errorId = (int)EnumErroresEmpresa.errorEditandoEmpresa;
                    errorDesc = errorDescBD ?? "Error al actualizar la empresa";
                    return res;
                }

                res.Empresa = new Core.Entidades.Empresa { Guid = guid, Nombre = req.Nombre, Telefono = req.Telefono, Correo = req.Correo, Estado = true };
                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.errorEditandoEmpresa, Mensaje = ex.Message });
                errorId = (int)EnumErroresEmpresa.errorEditandoEmpresa;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        public ResBase Eliminar(Guid guid)
        {
            var res = new ResBase { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_ELIMINAR_EMPRESA(guid, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (errorIdBD.HasValue && errorIdBD.Value != 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.errorEliminandoEmpresa, Mensaje = errorDescBD ?? "Error al eliminar la empresa" });
                    errorId = (int)EnumErroresEmpresa.errorEliminandoEmpresa;
                    errorDesc = errorDescBD ?? "Error al eliminar la empresa";
                    return res;
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.errorEliminandoEmpresa, Mensaje = ex.Message });
                errorId = (int)EnumErroresEmpresa.errorEliminandoEmpresa;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, guid, res);
            }

            return res;
        }

        private void bitacorear(enumBitacora tipo, int errorId, string errorDesc, object req, object res)
        {
            try
            {
                ReqBitacorear reqBit = new ReqBitacorear();
                reqBit.bitacora = new Bitacora
                {
                    clase       = GetType().Name,
                    metodo      = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name,
                    tipo        = tipo,
                    codigoError = errorId,
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
