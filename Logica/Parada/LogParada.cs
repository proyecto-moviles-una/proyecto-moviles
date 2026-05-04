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
                //Validiciones 
                // Valida que el nombre no venga vacío
                // o nulo antes de procesar la creación
                if (string.IsNullOrEmpty(req.Nombre))
                {
                    res.error.Add(new Error
                    {
                        Codigo = (int)EnumErroresParada.nombreFaltante,
                        Mensaje = "El nombre es obligatorio"
                    });
                }
                // Verifica que la descripción haya sido ingresada correctamente

                if (string.IsNullOrEmpty(req.Descripcion))
                {
                    res.error.Add(new Error
                    {
                        Codigo = (int)EnumErroresParada.descripcionFaltante,
                        Mensaje = "La descripción es obligatoria"
                    });
                }
                // Se realizan validaciones de los datos antes de
                // proceder con la inserción en base de datos
                if (req.Latitud < -90 || req.Latitud > 90)
                {
                    res.error.Add(new Error
                    {
                        Codigo = (int)EnumErroresParada.latitudInvalida,
                        Mensaje = "Latitud inválida"
                    });
                }

                if (req.Longitud < -180 || req.Longitud > 180)
                {
                    res.error.Add(new Error
                    {
                        Codigo = (int)EnumErroresParada.longitudInvalida,
                        Mensaje = "Longitud inválida"
                    });
                }
                //// Si existen errores de validación, se detiene el proceso y se retorna la respuesta
                if (res.error.Any())
                {
                    return res;
                }

                // Inserta la nueva parada en la base de datos y mapea el resultado a la entidad Core
                // Inserción en base de datos
                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {

                    // Validar duplicado
                    if (db.TB_PARADAs.Any(x => x.NOMBRE == req.Nombre && x.ESTADO == true))
                    {
                        res.error.Add(new Error
                        {
                            Codigo = (int)EnumErroresParada.paradaDuplicada,
                            Mensaje = "Ya existe una parada con ese nombre"
                        });

                        return res;
                    }

                    // Genera un identificador único (GUID) para la nueva parada
                    Guid guid = Guid.NewGuid();

                    TB_PARADA nueva = new TB_PARADA
                    {
                        // Asigna el GUID generado a la entidad de base de datos
                        GUID_PARADA = guid,
                        NOMBRE = req.Nombre,
                        DESCRIPCION = req.Descripcion,
                        LATITUD = req.Latitud,
                        LONGITUD = req.Longitud,
                        ESTADO = true,
                        FECHA_REGISTRO = DateTime.Now
                    };

                    db.TB_PARADAs.InsertOnSubmit(nueva);
                    db.SubmitChanges();

                    // Mapea los datos de la entidad de base de datos a la
                    // entidad del Core para la respuesta
                    res.parada = new Core.Entidades.Parada
                    {
                        guid = nueva.GUID_PARADA,
                        Nombre = nueva.NOMBRE,
                        Descripcion = nueva.DESCRIPCION,
                        Latitud = nueva.LATITUD,
                        Longitud = nueva.LONGITUD
                    };

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
                // bitácora
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
                    // Obtiene únicamente las paradas activas
                    var lista = db.TB_PARADAs
                                  .Where(p => p.ESTADO == true)
                                  .ToList();

                    // Mapeo de BD a Core
                    res.paradas = lista.Select(p => new Core.Entidades.Parada
                    {
                        guid = p.GUID_PARADA,
                        Nombre = p.NOMBRE,
                        Descripcion = p.DESCRIPCION,
                        Latitud = p.LATITUD,
                        Longitud = p.LONGITUD
                    }).ToList();

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

        // Método para obtener una parada por su GUID
        public ResCrearParada ObtenerPorGuid(Guid guid)
        {
            ResCrearParada res = new ResCrearParada();
            res.resultado = false;
            res.error = new List<Error>();

            try
            {
                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {
                    var p = db.TB_PARADAs
                              .FirstOrDefault(x => x.GUID_PARADA == guid);

                    if (p == null)
                    {
                        res.error.Add(new Error
                        {
                            Codigo = (int)EnumErroresParada.paradaNoEncontrada,
                            Mensaje = "Parada no encontrada"
                        });
                        return res;
                    }

                    res.parada = new Core.Entidades.Parada
                    {
                        guid = p.GUID_PARADA,
                        Nombre = p.NOMBRE,
                        Descripcion = p.DESCRIPCION,
                        Latitud = p.LATITUD,
                        Longitud = p.LONGITUD
                    };

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
        // Método para editar una parada existente utilizando su GUID
        public ResCrearParada Editar(Guid guid, ReqCrearParada req)
        {
            ResCrearParada res = new ResCrearParada();
            res.resultado = false;
            res.error = new List<Error>();

            try
            {
                using (ConexionLinqDataContext db = new ConexionLinqDataContext())
                {
                    var p = db.TB_PARADAs
                              .FirstOrDefault(x => x.GUID_PARADA == guid);

                    if (p == null)
                    {
                        res.error.Add(new Error
                        {
                            Codigo = (int)EnumErroresParada.paradaNoEncontrada,
                            Mensaje = "Parada no encontrada"
                        });
                        return res;
                    }

                    p.NOMBRE = req.Nombre;
                    p.DESCRIPCION = req.Descripcion;
                    p.LATITUD = req.Latitud;
                    p.LONGITUD = req.Longitud;

                    db.SubmitChanges();

                    res.parada = new Core.Entidades.Parada
                    {
                        guid = guid,
                        Nombre = p.NOMBRE,
                        Descripcion = p.DESCRIPCION,
                        Latitud = p.LATITUD,
                        Longitud = p.LONGITUD
                    };

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

        // Método para eliminar una parada utilizando su GUID

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
                              .FirstOrDefault(x => x.GUID_PARADA == guid);

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
    }
}
