using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum;
using Core.Enum.Ruta;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Ruta
{
    public class LogRuta
    {
        public ResCrearRuta Crear(ReqCrearRuta req)
        {
            var res = new ResCrearRuta { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(req.Nombre))
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresRuta.nombreFaltante, Mensaje = "El nombre es obligatorio" });
                    errorId = (int)EnumErroresRuta.nombreFaltante;
                    errorDesc = "El nombre es obligatorio";
                    return res;
                }

                System.Nullable<System.Guid> guidRuta = null;
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_INGRESAR_RUTA(
                        req.GuidEmpresa,
                        req.GuidZona,
                        req.NumeroRuta,
                        req.Nombre,
                        req.HoraInicio,
                        req.HoraFin,
                        1,
                        ref guidRuta,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (guidRuta == null || guidRuta == Guid.Empty)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorCreandoRuta, Mensaje = errorDescBD ?? "Error al crear la ruta" });
                    errorId = (int)EnumErroresRuta.errorCreandoRuta;
                    errorDesc = errorDescBD ?? "Error al crear la ruta";
                    return res;
                }

                res.Ruta = new Core.Entidades.Ruta
                {
                    Guid = guidRuta.Value,
                    GuidEmpresa = req.GuidEmpresa,
                    GuidZona = req.GuidZona,
                    NumeroRuta = req.NumeroRuta,
                    Nombre = req.Nombre,
                    HoraInicio = req.HoraInicio,
                    HoraFin = req.HoraFin,
                    EstadoServicio = 1
                };
                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorCreandoRuta, Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "") });
                errorId = (int)EnumErroresRuta.errorCreandoRuta;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        public ResListarRutas Listar(bool soloActivas = true)
        {
            var res = new ResListarRutas { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    res.Rutas = db.SP_OBTENER_RUTAS(soloActivas).Select(x => new Core.Entidades.Ruta
                    {
                        Guid = x.GUID_RUTA,
                        GuidEmpresa = x.GUID_EMPRESA,
                        NombreEmpresa = x.NOMBRE_EMPRESA,
                        GuidZona = x.GUID_ZONA,
                        NombreZona = x.NOMBRE_ZONA,
                        NumeroRuta = x.NUMERO_RUTA,
                        Nombre = x.NOMBRE,
                        TarifaActual = x.TARIFA_ACTUAL,
                        HoraInicio = x.HORA_INICIO,
                        HoraFin = x.HORA_FIN,
                        EstadoServicio = x.ESTADO_SERVICIO
                    }).ToList();
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorCreandoRuta, Mensaje = ex.Message });
                errorId = (int)EnumErroresRuta.errorCreandoRuta;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, soloActivas, res);
            }

            return res;
        }

        public ResCrearRuta ObtenerPorGuid(Guid guid)
        {
            var res = new ResCrearRuta { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    var r = db.SP_OBTENER_RUTA_POR_GUID(guid).FirstOrDefault();

                    if (r == null)
                    {
                        res.error.Add(new Error { Codigo = (int)EnumErroresRuta.rutaNoEncontrada, Mensaje = "Ruta no encontrada" });
                        errorId = (int)EnumErroresRuta.rutaNoEncontrada;
                        errorDesc = "Ruta no encontrada";
                        return res;
                    }

                    res.Ruta = new Core.Entidades.Ruta
                    {
                        Guid = r.GUID_RUTA,
                        GuidEmpresa = r.GUID_EMPRESA,
                        NombreEmpresa = r.NOMBRE_EMPRESA,
                        GuidZona = r.GUID_ZONA,
                        NombreZona = r.NOMBRE_ZONA,
                        NumeroRuta = r.NUMERO_RUTA,
                        Nombre = r.NOMBRE,
                        TarifaActual = r.TARIFA_ACTUAL,
                        GuidTarifa = r.GUID_TARIFA,
                        HoraInicio = r.HORA_INICIO,
                        HoraFin = r.HORA_FIN,
                        EstadoServicio = r.ESTADO_SERVICIO
                    };
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorCreandoRuta, Mensaje = ex.Message });
                errorId = (int)EnumErroresRuta.errorCreandoRuta;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, guid, res);
            }

            return res;
        }

        public ResCrearRuta Editar(Guid guid, ReqCrearRuta req)
        {
            var res = new ResCrearRuta { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(req.Nombre))
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresRuta.nombreFaltante, Mensaje = "El nombre es obligatorio" });
                    errorId = (int)EnumErroresRuta.nombreFaltante;
                    errorDesc = "El nombre es obligatorio";
                    return res;
                }

                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_ACTUALIZAR_RUTA(
                        guid,
                        req.Nombre,
                        req.NumeroRuta,
                        req.HoraInicio,
                        req.HoraFin,
                        null,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (errorIdBD.HasValue && errorIdBD.Value != 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorEditandoRuta, Mensaje = errorDescBD ?? "Error al actualizar la ruta" });
                    errorId = (int)EnumErroresRuta.errorEditandoRuta;
                    errorDesc = errorDescBD ?? "Error al actualizar la ruta";
                    return res;
                }

                res.Ruta = new Core.Entidades.Ruta
                {
                    Guid = guid,
                    GuidEmpresa = req.GuidEmpresa,
                    GuidZona = req.GuidZona,
                    NumeroRuta = req.NumeroRuta,
                    Nombre = req.Nombre,
                    HoraInicio = req.HoraInicio,
                    HoraFin = req.HoraFin
                };
                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorEditandoRuta, Mensaje = ex.Message });
                errorId = (int)EnumErroresRuta.errorEditandoRuta;
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
                    db.SP_DESACTIVAR_RUTA(guid, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (errorIdBD.HasValue && errorIdBD.Value != 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorEliminandoRuta, Mensaje = errorDescBD ?? "Error al desactivar la ruta" });
                    errorId = (int)EnumErroresRuta.errorEliminandoRuta;
                    errorDesc = errorDescBD ?? "Error al desactivar la ruta";
                    return res;
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorEliminandoRuta, Mensaje = ex.Message });
                errorId = (int)EnumErroresRuta.errorEliminandoRuta;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, guid, res);
            }

            return res;
        }

        public ResAsociarParadaRuta AsociarParada(ReqAsociarParadaRuta req)
        {
            var res = new ResAsociarParadaRuta { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                System.Nullable<System.Guid> guidRutaParada = null;
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_ASOCIAR_PARADA_RUTA(
                        req.GuidRuta,
                        req.GuidParada,
                        req.Orden,
                        ref guidRutaParada,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (guidRutaParada == null || guidRutaParada == Guid.Empty)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorCreandoRuta, Mensaje = errorDescBD ?? "Error al asociar la parada" });
                    errorId = (int)EnumErroresRuta.errorCreandoRuta;
                    errorDesc = errorDescBD ?? "Error al asociar la parada";
                    return res;
                }

                res.GuidRutaParada = guidRutaParada;
                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorCreandoRuta, Mensaje = ex.Message });
                errorId = (int)EnumErroresRuta.errorCreandoRuta;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        public ResBase DesasociarParada(ReqDesasociarParadaRuta req)
        {
            var res = new ResBase { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (req == null || req.GuidRuta == Guid.Empty || req.GuidParada == Guid.Empty)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorDesasociandoParadaRuta, Mensaje = "La ruta y la parada son obligatorias" });
                    errorId = (int)EnumErroresRuta.errorDesasociandoParadaRuta;
                    errorDesc = "La ruta y la parada son obligatorias";
                    return res;
                }

                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_DESASOCIAR_PARADA_RUTA(
                        req.GuidRuta,
                        req.GuidParada,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (errorIdBD.HasValue && errorIdBD.Value != 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorDesasociandoParadaRuta, Mensaje = errorDescBD ?? "Error al desasociar la parada" });
                    errorId = (int)EnumErroresRuta.errorDesasociandoParadaRuta;
                    errorDesc = errorDescBD ?? "Error al desasociar la parada";
                    return res;
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresRuta.errorDesasociandoParadaRuta, Mensaje = ex.Message });
                errorId = (int)EnumErroresRuta.errorDesasociandoParadaRuta;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, req, res);
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
