using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Entities;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum.Historial;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Historial
{
    public class LogHistorial
    {
        public ResCrearHistorial Crear(ReqCrearHistorial req)
        {
            ResCrearHistorial res = new ResCrearHistorial();

            res.resultado = false;
            res.error = new List<Error>();

            try
            {
                if (req.GuidUsuario == Guid.Empty)
                {
                    res.error.Add(new Error
                    {
                        Codigo = (int)ErroresHistorial.usuarioFaltante,
                        Mensaje = "Usuario obligatorio"
                    });
                }

                if (req.GuidRuta == Guid.Empty)
                {
                    res.error.Add(new Error
                    {
                        Codigo = (int)ErroresHistorial.rutaFaltante,
                        Mensaje = "Ruta obligatoria"
                    });
                }

                if (res.error.Any())
                    return res;

                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {
                    Guid? guidHistorial = null;
                    int? idReturn = null;
                    int? errorId = null;
                    string errorDescripcion = null;

                    db.SP_REGISTRAR_HISTORIAL(
                        req.GuidUsuario,
                        req.GuidRuta,
                        ref guidHistorial,
                        ref idReturn,
                        ref errorId,
                        ref errorDescripcion
                    );

                    if (idReturn.HasValue && idReturn.Value > 0)
                    {
                        res.historial = new HistorialViaje
                        {
                            GuidHistorial = guidHistorial ?? Guid.Empty,
                            GuidUsuario = req.GuidUsuario,
                            GuidRuta = req.GuidRuta
                        };

                        res.resultado = true;
                        res.error = null;
                    }
                    else
                    {
                        res.error.Add(new Error
                        {
                            Codigo = errorId ?? (int)ErroresHistorial.errorRegistrandoHistorial,
                            Mensaje = string.IsNullOrEmpty(errorDescripcion)
                                ? "Error al registrar historial"
                                : errorDescripcion
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                res.error.Add(new Error
                {
                    Codigo = (int)ErroresHistorial.errorRegistrandoHistorial,
                    Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                });
            }
            finally
            {
                // bitácora pendiente de integración
            }

            return res;
        }

        public ResListarHistorial ListarPorUsuario(Guid guidUsuario)
        {
            ResListarHistorial res = new ResListarHistorial();

            res.resultado = false;
            res.error = new List<Error>();

            try
            {
                if (guidUsuario == Guid.Empty)
                {
                    res.error.Add(new Error
                    {
                        Codigo = (int)ErroresHistorial.usuarioFaltante,
                        Mensaje = "Usuario obligatorio"
                    });
                    return res;
                }

                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {
                    var lista = db.SP_OBTENER_HISTORIAL_POR_USUARIO(guidUsuario).ToList();

                    res.historial = lista.Select(x => new HistorialViaje
                    {
                        GuidHistorial = x.GUID_HISTORIAL,
                        GuidRuta = x.GUID_RUTA,
                        GuidUsuario = guidUsuario,
                        NombreRuta = x.NOMBRE_RUTA,
                        Tarifa = x.TARIFA_CONSULTADA ?? 0,
                        FechaConsulta = x.FECHA_CONSULTA
                    }).ToList();

                    res.resultado = true;
                    res.error = null;
                }
            }
            catch (Exception ex)
            {
                res.error.Add(new Error
                {
                    Codigo = (int)ErroresHistorial.errorConsultandoHistorial,
                    Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                });
            }
            finally
            {
                // bitácora pendiente de integración
            }

            return res;
        }
    }
}
