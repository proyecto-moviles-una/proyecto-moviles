using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum.Parada;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Parada
{
    public class LogParada
    {
        public ResCrearParada Crear(ReqCrearParada req)
        {
            ResCrearParada res = new ResCrearParada();
            res.resultado = false;
            res.error = new List<Error>();

            try
            {
                ValidarDatosParada(req, res.error);
                if (res.error.Any())
                {
                    return res;
                }

                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {
                    if (ExisteDuplicado(db, req.Nombre, null))
                    {
                        res.error.Add(new Error
                        {
                            Codigo = (int)EnumErroresParada.paradaDuplicada,
                            Mensaje = "Ya existe una parada con ese nombre"
                        });

                        return res;
                    }

                    Guid guid = Guid.NewGuid();

                    TB_PARADA nueva = new TB_PARADA
                    {
                        GUID_PARADA = guid,
                        NOMBRE = req.Nombre.Trim(),
                        DESCRIPCION = req.Descripcion.Trim(),
                        LATITUD = req.Latitud,
                        LONGITUD = req.Longitud,
                        ESTADO = true,
                        FECHA_REGISTRO = DateTime.Now
                    };

                    db.TB_PARADAs.InsertOnSubmit(nueva);
                    db.SubmitChanges();

                    res.parada = MapearParada(nueva);
                    res.resultado = true;
                    res.error = null;
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.error.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.errorCreandoParada,
                    Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                });
            }
            finally
            {
                // bitácora pendiente de integración
            }

            return res;
        }

        public ResListarParadas Listar()
        {
            ResListarParadas res = new ResListarParadas();
            res.resultado = false;
            res.error = new List<Error>();

            try
            {
                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {
                    var lista = db.TB_PARADAs
                                  .Where(p => p.ESTADO == true)
                                  .ToList();

                    res.paradas = lista.Select(MapearParada).ToList();
                    res.resultado = true;
                    res.error = null;
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.error.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.paradaNoEncontrada,
                    Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                });
            }
            finally
            {
                // bitácora pendiente de integración
            }

            return res;
        }

        public ResListarParadas ListarCercanas(decimal latitud, decimal longitud, decimal radioKm)
        {
            ResListarParadas res = new ResListarParadas();
            res.resultado = false;
            res.error = new List<Error>();

            try
            {
                if (latitud < -90 || latitud > 90)
                {
                    res.error.Add(CrearError(EnumErroresParada.latitudInvalida, "Latitud invalida"));
                    return res;
                }

                if (longitud < -180 || longitud > 180)
                {
                    res.error.Add(CrearError(EnumErroresParada.longitudInvalida, "Longitud invalida"));
                    return res;
                }

                if (radioKm <= 0)
                {
                    res.error.Add(CrearError(EnumErroresParada.paradaNoEncontrada, "El radio debe ser mayor a cero"));
                    return res;
                }

                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {
                    res.paradas = db.TB_PARADAs
                                    .Where(p => p.ESTADO == true)
                                    .AsEnumerable()
                                    .Select(p =>
                                    {
                                        Core.Entidades.Parada parada = MapearParada(p);
                                        parada.DistanciaKm = CalcularDistanciaKm(
                                            latitud,
                                            longitud,
                                            p.LATITUD,
                                            p.LONGITUD);
                                        return parada;
                                    })
                                    .Where(p => p.DistanciaKm.HasValue && p.DistanciaKm.Value <= radioKm)
                                    .OrderBy(p => p.DistanciaKm)
                                    .ThenBy(p => p.Nombre)
                                    .ToList();

                    res.resultado = true;
                    res.error = null;
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.error.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.paradaNoEncontrada,
                    Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                });
            }
            finally
            {
                // bitÃ¡cora pendiente de integraciÃ³n
            }

            return res;
        }

        public ResListarParadas ListarPorRuta(Guid guidRuta)
        {
            ResListarParadas res = new ResListarParadas();
            res.resultado = false;
            res.error = new List<Error>();

            try
            {
                if (guidRuta == Guid.Empty)
                {
                    res.error.Add(new Error
                    {
                        Codigo = (int)EnumErroresParada.paradaNoEncontrada,
                        Mensaje = "La ruta es obligatoria"
                    });
                    return res;
                }

                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {
                    res.paradas = db.SP_OBTENER_PARADAS_POR_RUTA(guidRuta)
                                    .Select(p => new Core.Entidades.Parada
                                    {
                                        guid = p.GUID_PARADA,
                                        Nombre = p.NOMBRE,
                                        Descripcion = p.DESCRIPCION,
                                        Latitud = p.LATITUD ?? 0,
                                        Longitud = p.LONGITUD ?? 0,
                                        Orden = p.ORDEN
                                    })
                                    .ToList();

                    res.resultado = true;
                    res.error = null;
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.error.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.paradaNoEncontrada,
                    Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                });
            }
            finally
            {
                // bitácora pendiente de integración
            }

            return res;
        }

        public ResListarParadas ListarPorRuta(string guidRuta)
        {
            Guid guid;
            if (!Guid.TryParse(guidRuta, out guid))
            {
                ResListarParadas res = new ResListarParadas();
                res.resultado = false;
                res.error = new List<Error>
                {
                    CrearError(EnumErroresParada.paradaNoEncontrada, "Guid de ruta invalido")
                };
                return res;
            }

            return ListarPorRuta(guid);
        }

        public ResListarRutas ListarRutasPorParada(Guid guidParada)
        {
            ResListarRutas res = new ResListarRutas();
            res.resultado = false;
            res.error = new List<Error>();
            res.Rutas = new List<Core.Entidades.Ruta>();

            try
            {
                if (guidParada == Guid.Empty)
                {
                    res.error.Add(CrearError(EnumErroresParada.paradaNoEncontrada, "La parada es obligatoria"));
                    return res;
                }

                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {
                    bool existeParada = db.TB_PARADAs.Any(p => p.GUID_PARADA == guidParada && p.ESTADO == true);
                    if (!existeParada)
                    {
                        res.error.Add(CrearError(EnumErroresParada.paradaNoEncontrada, "Parada no encontrada"));
                        return res;
                    }

                    var rutas = db.SP_OBTENER_RUTAS(true).ToList();
                    foreach (var ruta in rutas)
                    {
                        bool asociada = db.SP_OBTENER_PARADAS_POR_RUTA(ruta.GUID_RUTA)
                                          .Any(p => p.GUID_PARADA == guidParada);

                        if (!asociada)
                        {
                            continue;
                        }

                        res.Rutas.Add(new Core.Entidades.Ruta
                        {
                            Guid = ruta.GUID_RUTA,
                            GuidEmpresa = ruta.GUID_EMPRESA,
                            NombreEmpresa = ruta.NOMBRE_EMPRESA,
                            GuidZona = ruta.GUID_ZONA,
                            NombreZona = ruta.NOMBRE_ZONA,
                            NumeroRuta = ruta.NUMERO_RUTA,
                            Nombre = ruta.NOMBRE,
                            TarifaActual = ruta.TARIFA_ACTUAL,
                            HoraInicio = ruta.HORA_INICIO,
                            HoraFin = ruta.HORA_FIN,
                            EstadoServicio = ruta.ESTADO_SERVICIO
                        });
                    }

                    res.Rutas = res.Rutas
                                   .OrderBy(r => r.NumeroRuta)
                                   .ThenBy(r => r.Nombre)
                                   .ToList();
                    res.resultado = true;
                    res.error = null;
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.error.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.paradaNoEncontrada,
                    Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                });
            }
            finally
            {
                // bitÃ¡cora pendiente de integraciÃ³n
            }

            return res;
        }

        public ResListarRutas ListarRutasPorParada(string guidParada)
        {
            Guid guid;
            if (!Guid.TryParse(guidParada, out guid))
            {
                ResListarRutas res = new ResListarRutas();
                res.resultado = false;
                res.error = new List<Error>
                {
                    CrearError(EnumErroresParada.paradaNoEncontrada, "Guid de parada invalido")
                };
                return res;
            }

            return ListarRutasPorParada(guid);
        }

        public ResCrearParada ObtenerPorGuid(Guid guid)
        {
            ResCrearParada res = new ResCrearParada();
            res.resultado = false;
            res.error = new List<Error>();

            try
            {
                if (guid == Guid.Empty)
                {
                    res.error.Add(new Error
                    {
                        Codigo = (int)EnumErroresParada.paradaNoEncontrada,
                        Mensaje = "Parada no encontrada"
                    });
                    return res;
                }

                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {
                    var p = db.TB_PARADAs
                              .FirstOrDefault(x => x.GUID_PARADA == guid && x.ESTADO == true);

                    if (p == null)
                    {
                        res.error.Add(new Error
                        {
                            Codigo = (int)EnumErroresParada.paradaNoEncontrada,
                            Mensaje = "Parada no encontrada"
                        });
                        return res;
                    }

                    res.parada = MapearParada(p);
                    res.resultado = true;
                    res.error = null;
                }
            }
            catch (Exception)
            {
                res.error.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.errorCreandoParada,
                    Mensaje = "Error al obtener la parada"
                });
            }
            finally
            {
                // bitácora pendiente de integración
            }

            return res;
        }

        public ResCrearParada ObtenerPorGuid(string guid)
        {
            Guid guidParada;
            if (!Guid.TryParse(guid, out guidParada))
            {
                ResCrearParada res = new ResCrearParada();
                res.resultado = false;
                res.error = new List<Error>
                {
                    CrearError(EnumErroresParada.paradaNoEncontrada, "Guid de parada invalido")
                };
                return res;
            }

            return ObtenerPorGuid(guidParada);
        }

        public ResCrearParada Editar(Guid guid, ReqCrearParada req)
        {
            ResCrearParada res = new ResCrearParada();
            res.resultado = false;
            res.error = new List<Error>();

            try
            {
                ValidarDatosParada(req, res.error);
                if (res.error.Any())
                {
                    return res;
                }

                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {
                    var p = db.TB_PARADAs
                              .FirstOrDefault(x => x.GUID_PARADA == guid && x.ESTADO == true);

                    if (p == null)
                    {
                        res.error.Add(new Error
                        {
                            Codigo = (int)EnumErroresParada.paradaNoEncontrada,
                            Mensaje = "Parada no encontrada"
                        });
                        return res;
                    }

                    if (ExisteDuplicado(db, req.Nombre, guid))
                    {
                        res.error.Add(new Error
                        {
                            Codigo = (int)EnumErroresParada.paradaDuplicada,
                            Mensaje = "Ya existe una parada con ese nombre"
                        });
                        return res;
                    }

                    p.NOMBRE = req.Nombre.Trim();
                    p.DESCRIPCION = req.Descripcion.Trim();
                    p.LATITUD = req.Latitud;
                    p.LONGITUD = req.Longitud;

                    db.SubmitChanges();

                    res.parada = MapearParada(p);
                    res.resultado = true;
                    res.error = null;
                }
            }
            catch (Exception)
            {
                res.error.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.errorCreandoParada,
                    Mensaje = "Error al editar la parada"
                });
            }
            finally
            {
                // bitácora pendiente de integración
            }

            return res;
        }

        public ResEditarParada Editar(ReqEditarParada req)
        {
            ResEditarParada res = new ResEditarParada();
            res.resultado = false;
            res.error = new List<Error>();

            Guid guid;
            if (req == null || !Guid.TryParse(req.Guid, out guid))
            {
                res.error.Add(CrearError(EnumErroresParada.paradaNoEncontrada, "Guid de parada invalido"));
                return res;
            }

            ReqCrearParada reqCrear = new ReqCrearParada();
            reqCrear.Nombre = req.Nombre;
            reqCrear.Descripcion = req.Descripcion;
            reqCrear.Latitud = req.Latitud;
            reqCrear.Longitud = req.Longitud;

            ResCrearParada resCrear = Editar(guid, reqCrear);
            res.resultado = resCrear.resultado;
            res.error = resCrear.error;
            res.parada = resCrear.parada;

            return res;
        }

        public ResBase Eliminar(Guid guid)
        {
            ResBase res = new ResBase();
            res.resultado = false;
            res.error = new List<Error>();

            try
            {
                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {
                    var p = db.TB_PARADAs
                              .FirstOrDefault(x => x.GUID_PARADA == guid && x.ESTADO == true);

                    if (p == null)
                    {
                        res.error.Add(new Error
                        {
                            Codigo = (int)EnumErroresParada.paradaNoEncontrada,
                            Mensaje = "Parada no encontrada"
                        });
                        return res;
                    }

                    p.ESTADO = false;
                    db.SubmitChanges();

                    res.resultado = true;
                    res.error = null;
                }
            }
            catch (Exception)
            {
                res.error.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.errorCreandoParada,
                    Mensaje = "Error al eliminar la parada"
                });
            }
            finally
            {
                // bitácora pendiente de integración
            }

            return res;
        }

        public ResEliminarParada Eliminar(ReqEliminarParada req)
        {
            ResEliminarParada res = new ResEliminarParada();
            res.resultado = false;
            res.error = new List<Error>();

            Guid guid;
            if (req == null || !Guid.TryParse(req.Guid, out guid))
            {
                res.error.Add(CrearError(EnumErroresParada.paradaNoEncontrada, "Guid de parada invalido"));
                return res;
            }

            ResBase resBase = Eliminar(guid);
            res.resultado = resBase.resultado;
            res.error = resBase.error;

            return res;
        }

        private Error CrearError(EnumErroresParada codigo, string mensaje)
        {
            return new Error
            {
                Codigo = (int)codigo,
                Mensaje = mensaje
            };
        }

        private void ValidarDatosParada(ReqCrearParada req, List<Error> errores)
        {
            if (req == null)
            {
                errores.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.nombreFaltante,
                    Mensaje = "Los datos de la parada son obligatorios"
                });
                return;
            }

            if (string.IsNullOrWhiteSpace(req.Nombre))
            {
                errores.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.nombreFaltante,
                    Mensaje = "El nombre es obligatorio"
                });
            }

            if (string.IsNullOrWhiteSpace(req.Descripcion))
            {
                errores.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.descripcionFaltante,
                    Mensaje = "La descripción es obligatoria"
                });
            }

            if (req.Latitud < -90 || req.Latitud > 90)
            {
                errores.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.latitudInvalida,
                    Mensaje = "Latitud inválida"
                });
            }

            if (req.Longitud < -180 || req.Longitud > 180)
            {
                errores.Add(new Error
                {
                    Codigo = (int)EnumErroresParada.longitudInvalida,
                    Mensaje = "Longitud inválida"
                });
            }
        }

        private bool ExisteDuplicado(ConexionLinqDataContext db, string nombre, Guid? guidExcluir)
        {
            string nombreNormalizado = nombre.Trim();

            if (guidExcluir.HasValue)
            {
                Guid guid = guidExcluir.Value;

                return db.TB_PARADAs.Any(x =>
                    x.NOMBRE == nombreNormalizado &&
                    x.ESTADO == true &&
                    x.GUID_PARADA != guid);
            }

            return db.TB_PARADAs.Any(x =>
                x.NOMBRE == nombreNormalizado &&
                x.ESTADO == true);
        }

        private decimal CalcularDistanciaKm(decimal latitudOrigen, decimal longitudOrigen, decimal latitudDestino, decimal longitudDestino)
        {
            const double radioTierraKm = 6371.0088;

            double latOrigenRad = AGradosRadianes((double)latitudOrigen);
            double latDestinoRad = AGradosRadianes((double)latitudDestino);
            double diferenciaLatitud = AGradosRadianes((double)(latitudDestino - latitudOrigen));
            double diferenciaLongitud = AGradosRadianes((double)(longitudDestino - longitudOrigen));

            double a = Math.Sin(diferenciaLatitud / 2) * Math.Sin(diferenciaLatitud / 2) +
                       Math.Cos(latOrigenRad) * Math.Cos(latDestinoRad) *
                       Math.Sin(diferenciaLongitud / 2) * Math.Sin(diferenciaLongitud / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distancia = radioTierraKm * c;

            return Math.Round((decimal)distancia, 3);
        }

        private double AGradosRadianes(double grados)
        {
            return grados * Math.PI / 180;
        }

        private Core.Entidades.Parada MapearParada(TB_PARADA parada)
        {
            return new Core.Entidades.Parada
            {
                guid = parada.GUID_PARADA,
                Nombre = parada.NOMBRE,
                Descripcion = parada.DESCRIPCION,
                Latitud = parada.LATITUD,
                Longitud = parada.LONGITUD
            };
        }
    }
}
