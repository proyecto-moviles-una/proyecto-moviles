using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum.Horario;
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

            try
            {
                if (req.DiasServicio == 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresHorario.diasServicioInvalido, Mensaje = "Debe seleccionar al menos un día de servicio" });
                    return res;
                }

                System.Nullable<System.Guid> guidHorario = null;
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_INGRESAR_HORARIO(
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
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresHorario.errorCreandoHorario, Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "") });
            }

            return res;
        }

        public ResListarHorarios ListarPorRuta(Guid guidRuta)
        {
            var res = new ResListarHorarios { resultado = false, error = new List<Error>() };

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    res.Horarios = db.SP_OBTENER_HORARIOS_POR_RUTA(guidRuta).Select(x => new Core.Entidades.Horario
                    {
                        Guid = x.GUID_HORARIO,
                        GuidRuta = guidRuta,
                        HoraSalida = x.HORA_SALIDA,
                        DiasServicio = x.DIAS_SERVICIO,
                        Estado = x.ESTADO
                    }).ToList();
                }

                res.resultado = true;
                res.error = null;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresHorario.errorCreandoHorario, Mensaje = ex.Message });
            }

            return res;
        }
    }
}
