using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum;
using Core.Enum.Horario;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Horario
{
    public class LogHorario
    {
        public ResCrearHorario Crear(ReqCrearHorario req)
        {
            var res = new ResCrearHorario { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (req.DiasServicio == 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresHorario.diasServicioInvalido, Mensaje = "Debe seleccionar al menos un día de servicio" });
                    errorId = (int)EnumErroresHorario.diasServicioInvalido;
                    errorDesc = "Debe seleccionar al menos un día de servicio";
                    return res;
                }

                System.Nullable<System.Guid> guidHorario = null;
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_INGRESAR_HORARIO3(
                        req.GuidRuta,
                        req.HoraSalida,
                        req.DiasServicio,
                        ref guidHorario,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (guidHorario == null || guidHorario == Guid.Empty)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresHorario.errorCreandoHorario, Mensaje = errorDescBD ?? "Error al crear el horario" });
                    errorId = (int)EnumErroresHorario.errorCreandoHorario;
                    errorDesc = errorDescBD ?? "Error al crear el horario";
                    return res;
                }

                res.Horario = new Core.Entidades.Horario
                {
                    Guid = guidHorario.Value,
                    GuidRuta = req.GuidRuta,
                    HoraSalida = req.HoraSalida,
                    DiasServicio = req.DiasServicio,
                    Estado = true
                };
                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresHorario.errorCreandoHorario, Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "") });
                errorId = (int)EnumErroresHorario.errorCreandoHorario;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        public ResListarHorarios ListarPorRuta(Guid guidRuta)
        {
            var res = new ResListarHorarios { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    res.Horarios = db.SP_OBTENER_HORARIOS_POR_RUTA1(guidRuta).Select(x => new Core.Entidades.Horario
                    {
                        Guid = (Guid)x.GUID_HORARIO,
                        GuidRuta = guidRuta,
                        HoraSalida = (TimeSpan)x.HORA_SALIDA,
                        DiasServicio = (byte)x.DIAS_SERVICIO,
                        Estado = (bool)x.ESTADO
                    }).ToList();
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresHorario.errorCreandoHorario, Mensaje = ex.Message });
                errorId = (int)EnumErroresHorario.errorCreandoHorario;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, guidRuta, res);
            }

            return res;
        }

        public ResCrearHorario ObtenerPorGuid(Guid guid)
        {
            var res = new ResCrearHorario { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    var row = db.SP_OBTENER_HORARIO_POR_GUID(guid).FirstOrDefault();

                    if (row == null)
                    {
                        res.error.Add(new Error { Codigo = (int)EnumErroresHorario.horarioNoEncontrado, Mensaje = "Horario no encontrado" });
                        errorId = (int)EnumErroresHorario.horarioNoEncontrado;
                        errorDesc = "Horario no encontrado";
                        return res;
                    }

                    res.Horario = new Core.Entidades.Horario
                    {
                        Guid = row.GUID_HORARIO,
                        GuidRuta = row.GUID_RUTA,
                        HoraSalida = row.HORA_SALIDA,
                        DiasServicio = row.DIAS_SERVICIO,
                        Estado = row.ESTADO
                    };
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresHorario.horarioNoEncontrado, Mensaje = ex.Message });
                errorId = (int)EnumErroresHorario.horarioNoEncontrado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, guid, res);
            }

            return res;
        }

        public ResCrearHorario Editar(Guid guid, ReqCrearHorario req)
        {
            var res = new ResCrearHorario { resultado = false, error = new List<Error>() };
            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (req.DiasServicio == 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresHorario.diasServicioInvalido, Mensaje = "Debe seleccionar al menos un día de servicio" });
                    errorId = (int)EnumErroresHorario.diasServicioInvalido;
                    errorDesc = "Debe seleccionar al menos un día de servicio";
                    return res;
                }

                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_ACTUALIZAR_HORARIO2(guid, req.HoraSalida, req.DiasServicio, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (errorIdBD.HasValue && errorIdBD.Value != 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresHorario.errorEditandoHorario, Mensaje = errorDescBD ?? "Error al actualizar el horario" });
                    errorId = (int)EnumErroresHorario.errorEditandoHorario;
                    errorDesc = errorDescBD ?? "Error al actualizar el horario";
                    return res;
                }

                res.Horario = new Core.Entidades.Horario
                {
                    Guid = guid,
                    GuidRuta = req.GuidRuta,
                    HoraSalida = req.HoraSalida,
                    DiasServicio = req.DiasServicio,
                    Estado = true
                };
                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresHorario.errorEditandoHorario, Mensaje = ex.Message });
                errorId = (int)EnumErroresHorario.errorEditandoHorario;
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
                    db.SP_ELIMINAR_HORARIO2(guid, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (errorIdBD.HasValue && errorIdBD.Value != 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresHorario.errorEliminandoHorario, Mensaje = errorDescBD ?? "Error al eliminar el horario" });
                    errorId = (int)EnumErroresHorario.errorEliminandoHorario;
                    errorDesc = errorDescBD ?? "Error al eliminar el horario";
                    return res;
                }

                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresHorario.errorEliminandoHorario, Mensaje = ex.Message });
                errorId = (int)EnumErroresHorario.errorEliminandoHorario;
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
