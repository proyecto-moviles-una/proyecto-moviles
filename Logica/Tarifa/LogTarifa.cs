using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum;
using Core.Enum.Tarifa;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Tarifa
{
    public class LogTarifa
    {
        public ResCrearTarifa Crear(ReqCrearTarifa req)
        {
            var res = new ResCrearTarifa { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (req.Monto <= 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.montoInvalido, Mensaje = "El monto debe ser mayor a cero" });
                    errorId = (int)EnumErroresTarifa.montoInvalido;
                    errorDesc = "El monto debe ser mayor a cero";
                    return res;
                }

                if (req.FechaVigencia == default)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.fechaVigenciaInvalida, Mensaje = "La fecha de vigencia es obligatoria" });
                    errorId = (int)EnumErroresTarifa.fechaVigenciaInvalida;
                    errorDesc = "La fecha de vigencia es obligatoria";
                    return res;
                }

                System.Nullable<System.Guid> guidTarifa = null;
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_INGRESAR_TARIFA(
                        req.GuidRuta,
                        req.Monto,
                        req.FechaVigencia,
                        ref guidTarifa,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (guidTarifa == null || guidTarifa == Guid.Empty)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.errorCreandoTarifa, Mensaje = errorDescBD ?? "Error al crear la tarifa" });
                    errorId = (int)EnumErroresTarifa.errorCreandoTarifa;
                    errorDesc = errorDescBD ?? "Error al crear la tarifa";
                    return res;
                }

                res.Tarifa = new Core.Entidades.Tarifa
                {
                    Guid = guidTarifa.Value,
                    GuidRuta = req.GuidRuta,
                    Monto = req.Monto,
                    FechaVigencia = req.FechaVigencia,
                    Estado = true
                };
                res.resultado = true;
                res.error = null;


                // BEST-EFFORT: notifica a los usuarios que tienen esta ruta como favorita.
                // Si Firebase falla, la tarifa ya qued� guardada; el push no la invalida.
                Utilitarios.Utilitarios.EnviarPushFavoritosPorRuta(
                    req.GuidRuta,
                    "Tarifa actualizada",
                    string.Format("La tarifa de una ruta en tus favoritos cambi� a ?{0:N0}.", req.Monto));

                tipoBitacora = enumBitacora.exitoso;

            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.errorCreandoTarifa, Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "") });
                errorId = (int)EnumErroresTarifa.errorCreandoTarifa;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        public ResListarTarifas Listar(Guid guidRuta)
        {
            var res = new ResListarTarifas { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    res.Tarifas = db.SP_OBTENER_TARIFAS_POR_RUTA(guidRuta).Select(x => new Core.Entidades.Tarifa
                    {
                        Guid = x.GUID_TARIFA.GetValueOrDefault(),
                        GuidRuta = x.GUID_RUTA.GetValueOrDefault(),
                        Monto = x.MONTO.GetValueOrDefault(),
                        FechaVigencia = x.FECHA_VIGENCIA.GetValueOrDefault(),
                        Estado = x.ESTADO.GetValueOrDefault()
                    }).ToList();
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.errorCreandoTarifa, Mensaje = ex.Message });
                errorId = (int)EnumErroresTarifa.errorCreandoTarifa;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, guidRuta, res);
            }

            return res;
        }

        public ResCrearTarifa ObtenerPorGuid(Guid guid)
        {
            var res = new ResCrearTarifa { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    var row = db.SP_OBTENER_TARIFA_POR_GUID(guid).FirstOrDefault();

                    if (row == null)
                    {
                        res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.tarifaNoEncontrada, Mensaje = "Tarifa no encontrada" });
                        errorId = (int)EnumErroresTarifa.tarifaNoEncontrada;
                        errorDesc = "Tarifa no encontrada";
                        return res;
                    }

                    res.Tarifa = new Core.Entidades.Tarifa
                    {
                        Guid = row.GUID_TARIFA,
                        GuidRuta = row.GUID_RUTA,
                        Monto = row.MONTO,
                        FechaVigencia = row.FECHA_VIGENCIA,
                        Estado = row.ESTADO
                    };
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.errorCreandoTarifa, Mensaje = ex.Message });
                errorId = (int)EnumErroresTarifa.errorCreandoTarifa;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, guid, res);
            }

            return res;
        }

        public ResCrearTarifa Editar(Guid guid, ReqCrearTarifa req)
        {
            var res = new ResCrearTarifa { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (req.Monto <= 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.montoInvalido, Mensaje = "El monto debe ser mayor a cero" });
                    errorId = (int)EnumErroresTarifa.montoInvalido;
                    errorDesc = "El monto debe ser mayor a cero";
                    return res;
                }

                if (req.FechaVigencia == default)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.fechaVigenciaInvalida, Mensaje = "La fecha de vigencia es obligatoria" });
                    errorId = (int)EnumErroresTarifa.fechaVigenciaInvalida;
                    errorDesc = "La fecha de vigencia es obligatoria";
                    return res;
                }

                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_ACTUALIZAR_TARIFA(guid, req.Monto, req.FechaVigencia, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (errorIdBD.HasValue && errorIdBD.Value != 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.errorEditandoTarifa, Mensaje = errorDescBD ?? "Error al actualizar la tarifa" });
                    errorId = (int)EnumErroresTarifa.errorEditandoTarifa;
                    errorDesc = errorDescBD ?? "Error al actualizar la tarifa";
                    return res;
                }

                res.Tarifa = new Core.Entidades.Tarifa
                {
                    Guid = guid,
                    GuidRuta = req.GuidRuta,
                    Monto = req.Monto,
                    FechaVigencia = req.FechaVigencia,
                    Estado = true
                };
                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.errorEditandoTarifa, Mensaje = ex.Message });
                errorId = (int)EnumErroresTarifa.errorEditandoTarifa;
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
                    db.SP_ELIMINAR_TARIFA(guid, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (errorIdBD.HasValue && errorIdBD.Value != 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.errorEliminandoTarifa, Mensaje = errorDescBD ?? "Error al eliminar la tarifa" });
                    errorId = (int)EnumErroresTarifa.errorEliminandoTarifa;
                    errorDesc = errorDescBD ?? "Error al eliminar la tarifa";
                    return res;
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.errorEliminandoTarifa, Mensaje = ex.Message });
                errorId = (int)EnumErroresTarifa.errorEliminandoTarifa;
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
                var reqBit = new ReqBitacorear
                {
                    bitacora = new Bitacora
                    {
                        clase       = GetType().Name,
                        metodo      = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name,
                        tipo        = tipo,
                        codigoError = errorId,
                        descripcion = errorDesc,
                        request     = JsonConvert.SerializeObject(req),
                        response    = JsonConvert.SerializeObject(res)
                    }
                };
                Utilitarios.Utilitarios.bitacorear(reqBit);
            }
            catch { }
        }
    }
}
