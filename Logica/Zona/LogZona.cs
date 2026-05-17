using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum;
using Core.Enum.Zona;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Zona
{
    public class LogZona
    {
        public ResCrearZona Crear(ReqCrearZona req)
        {
            var res = new ResCrearZona { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(req.Nombre))
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresZona.nombreFaltante, Mensaje = "El nombre es obligatorio" });
                    errorId = (int)EnumErroresZona.nombreFaltante;
                    errorDesc = "El nombre es obligatorio";
                    return res;
                }

                System.Nullable<System.Guid> guidZona = null;
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_INGRESAR_ZONA(req.Nombre, req.Descripcion, ref guidZona, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (guidZona == null || guidZona == Guid.Empty)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresZona.errorCreandoZona, Mensaje = errorDescBD ?? "Error al crear la zona" });
                    errorId = (int)EnumErroresZona.errorCreandoZona;
                    errorDesc = errorDescBD ?? "Error al crear la zona";
                    return res;
                }

                res.Zona = new Core.Entidades.Zona { Guid = guidZona.Value, Nombre = req.Nombre, Descripcion = req.Descripcion, Estado = true };
                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresZona.errorCreandoZona, Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "") });
                errorId = (int)EnumErroresZona.errorCreandoZona;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        public ResListarZonas Listar()
        {
            var res = new ResListarZonas { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    res.Zonas = db.SP_OBTENER_ZONAS().Select(x => new Core.Entidades.Zona
                    {
                        Guid = x.GUID_ZONA,
                        Nombre = x.NOMBRE,
                        Descripcion = x.DESCRIPCION,
                        Estado = x.ESTADO
                    }).ToList();
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresZona.errorCreandoZona, Mensaje = ex.Message });
                errorId = (int)EnumErroresZona.errorCreandoZona;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, null, res);
            }

            return res;
        }

        public ResCrearZona ObtenerPorGuid(Guid guid)
        {
            var res = new ResCrearZona { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    var z = db.SP_OBTENER_ZONAS().FirstOrDefault(x => x.GUID_ZONA == guid);

                    if (z == null)
                    {
                        res.error.Add(new Error { Codigo = (int)EnumErroresZona.zonaNoEncontrada, Mensaje = "Zona no encontrada" });
                        errorId = (int)EnumErroresZona.zonaNoEncontrada;
                        errorDesc = "Zona no encontrada";
                        return res;
                    }

                    res.Zona = new Core.Entidades.Zona { Guid = z.GUID_ZONA, Nombre = z.NOMBRE, Descripcion = z.DESCRIPCION, Estado = z.ESTADO };
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresZona.errorCreandoZona, Mensaje = ex.Message });
                errorId = (int)EnumErroresZona.errorCreandoZona;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, guid, res);
            }

            return res;
        }

        public ResCrearZona Editar(Guid guid, ReqCrearZona req)
        {
            var res = new ResCrearZona { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(req.Nombre))
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresZona.nombreFaltante, Mensaje = "El nombre es obligatorio" });
                    errorId = (int)EnumErroresZona.nombreFaltante;
                    errorDesc = "El nombre es obligatorio";
                    return res;
                }

                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_ACTUALIZAR_ZONA(guid, req.Nombre, req.Descripcion, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (errorIdBD.HasValue && errorIdBD.Value != 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresZona.errorEditandoZona, Mensaje = errorDescBD ?? "Error al actualizar la zona" });
                    errorId = (int)EnumErroresZona.errorEditandoZona;
                    errorDesc = errorDescBD ?? "Error al actualizar la zona";
                    return res;
                }

                res.Zona = new Core.Entidades.Zona { Guid = guid, Nombre = req.Nombre, Descripcion = req.Descripcion, Estado = true };
                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresZona.errorEditandoZona, Mensaje = ex.Message });
                errorId = (int)EnumErroresZona.errorEditandoZona;
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
                    db.SP_ELIMINAR_ZONA(guid, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (errorIdBD.HasValue && errorIdBD.Value != 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresZona.errorEliminandoZona, Mensaje = errorDescBD ?? "Error al eliminar la zona" });
                    errorId = (int)EnumErroresZona.errorEliminandoZona;
                    errorDesc = errorDescBD ?? "Error al eliminar la zona";
                    return res;
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresZona.errorEliminandoZona, Mensaje = ex.Message });
                errorId = (int)EnumErroresZona.errorEliminandoZona;
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
