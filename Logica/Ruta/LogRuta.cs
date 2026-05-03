using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Ruta
{
    public class LogRuta
    {
        // =====================================================================
        // INSERTAR RUTA
        // =====================================================================
        public ResInsertarRuta insertar(ReqInsertarRuta req)
        {
            ResInsertarRuta res = new ResInsertarRuta();
            res.resultado = false;
            res.error = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(req.nombre))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.nombreRutaFaltante));
                    return res;
                }
                if (req.guidEmpresa == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidEmpresaFaltante));
                    return res;
                }

                System.Nullable<System.Guid> guidRuta = null;
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_INGRESAR_RUTA(
                        req.guidEmpresa,
                        req.guidZona,
                        req.numeroRuta,
                        req.nombre,
                        req.horaInicio,
                        req.horaFin,
                        req.estadoServicio,
                        ref guidRuta,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (guidRuta == null || guidRuta == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorInsertandoRuta));
                    errorId = (int)enumErrores.errorInsertandoRuta;
                    errorDesc = errorDescBD ?? enumErrores.errorInsertandoRuta.ToString();
                    return res;
                }

                res.resultado = true;
                res.guidRuta = guidRuta;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId = (int)enumErrores.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(null, tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // =====================================================================
        // OBTENER RUTA POR GUID
        // =====================================================================
        public ResObtenerRuta obtenerPorGuid(Guid guidRuta)
        {
            ResObtenerRuta res = new ResObtenerRuta();
            res.resultado = false;
            res.error = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (guidRuta == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidRutaFaltante));
                    return res;
                }

                List<SP_OBTENER_RUTASResult> lista;
                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    lista = linq.SP_OBTENER_RUTAS(null).ToList();
                }

                var sp = lista.FirstOrDefault(r => r.GUID_RUTA == guidRuta);

                if (sp == null)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorBaseDatos));
                    return res;
                }

                res.ruta = new Core.Entidades.Ruta
                {
                    guidRuta = sp.GUID_RUTA,
                    guidEmpresa = sp.GUID_EMPRESA,
                    nombreEmpresa = sp.NOMBRE_EMPRESA,
                    guidZona = sp.GUID_ZONA,
                    nombreZona = sp.NOMBRE_ZONA,
                    numeroRuta = sp.NUMERO_RUTA,
                    nombre = sp.NOMBRE,
                    tarifaActual = sp.TARIFA_ACTUAL,
                    horaInicio = sp.HORA_INICIO,
                    horaFin = sp.HORA_FIN,
                    estadoServicio = sp.ESTADO_SERVICIO,
                    fechaRegistro = sp.FECHA_REGISTRO
                };
                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId = (int)enumErrores.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(null, tipoBitacora, errorId, errorDesc, guidRuta, res);
            }

            return res;
        }

        // =====================================================================
        // OBTENER RUTAS
        // =====================================================================
        public ResObtenerRutas obtener(ReqObtenerRutas req)
        {
            ResObtenerRutas res = new ResObtenerRutas();
            res.resultado = false;
            res.error = new List<Error>();
            res.rutas = new List<Core.Entidades.Ruta>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                List<SP_OBTENER_RUTASResult> lista;
                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    lista = linq.SP_OBTENER_RUTAS(req.soloActivas).ToList();
                }

                res.rutas = factoriaRutas(lista);
                res.resultado = true;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId = (int)enumErrores.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(null, tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // =====================================================================
        // ACTUALIZAR RUTA
        // =====================================================================
        public ResActualizarRuta actualizar(ReqActualizarRuta req)
        {
            ResActualizarRuta res = new ResActualizarRuta();
            res.resultado = false;
            res.error = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (req.guidRuta == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidRutaFaltante));
                    return res;
                }
                if (string.IsNullOrWhiteSpace(req.nombre))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.nombreRutaFaltante));
                    return res;
                }

                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_ACTUALIZAR_RUTA(
                        req.guidRuta,
                        req.nombre,
                        req.numeroRuta,
                        req.horaInicio,
                        req.horaFin,
                        req.estadoServicio,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (idReturn > 0)
                {
                    res.resultado = true;
                    res.error = null;
                    tipoBitacora = enumBitacora.exitoso;
                }
                else
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorActualizandoRuta));
                    errorId = (int)enumErrores.errorActualizandoRuta;
                    errorDesc = errorDescBD ?? enumErrores.errorActualizandoRuta.ToString();
                }
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId = (int)enumErrores.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(null, tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // =====================================================================
        // DESACTIVAR RUTA
        // =====================================================================
        public ResDesactivarRuta desactivar(ReqDesactivarRuta req)
        {
            ResDesactivarRuta res = new ResDesactivarRuta();
            res.resultado = false;
            res.error = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (req.guidRuta == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidRutaFaltante));
                    return res;
                }

                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_DESACTIVAR_RUTA(
                        req.guidRuta,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (idReturn > 0)
                {
                    res.resultado = true;
                    res.error = null;
                    tipoBitacora = enumBitacora.exitoso;
                }
                else
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorDesactivandoRuta));
                    errorId = (int)enumErrores.errorDesactivandoRuta;
                    errorDesc = errorDescBD ?? enumErrores.errorDesactivandoRuta.ToString();
                }
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId = (int)enumErrores.errorNoControlado;
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
        private List<Core.Entidades.Ruta> factoriaRutas(List<SP_OBTENER_RUTASResult> lista)
        {
            List<Core.Entidades.Ruta> resultado = new List<Core.Entidades.Ruta>();
            foreach (var sp in lista)
            {
                resultado.Add(new Core.Entidades.Ruta
                {
                    guidRuta = sp.GUID_RUTA,
                    guidEmpresa = sp.GUID_EMPRESA,
                    nombreEmpresa = sp.NOMBRE_EMPRESA,
                    guidZona = sp.GUID_ZONA,
                    nombreZona = sp.NOMBRE_ZONA,
                    numeroRuta = sp.NUMERO_RUTA,
                    nombre = sp.NOMBRE,
                    tarifaActual = sp.TARIFA_ACTUAL,
                    horaInicio = sp.HORA_INICIO,
                    horaFin = sp.HORA_FIN,
                    estadoServicio = sp.ESTADO_SERVICIO,
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
                    clase = GetType().Name,
                    metodo = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name,
                    tipo = tipo,
                    errorId = errorId,
                    descripcion = errorDesc,
                    request = JsonConvert.SerializeObject(req),
                    response = JsonConvert.SerializeObject(res)
                };
                Utilitarios.Utilitarios.bitacorear(reqBit);
            }
            catch { }
        }
    }
}