using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Zona
{
    public class LogZona
    {
        // =====================================================================
        // INSERTAR ZONA
        // =====================================================================
        public ResInsertarZona insertar(ReqInsertarZona req)
        {
            ResInsertarZona res = new ResInsertarZona();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(req.nombre))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.nombreZonaFaltante));
                    return res;
                }

                System.Nullable<System.Guid> guidZona   = null;
                System.Nullable<int>         idReturn   = null;
                System.Nullable<int>         errorIdBD  = null;
                string                       errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_INGRESAR_ZONA(
                        req.nombre,
                        req.descripcion,
                        ref guidZona,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (guidZona == null || guidZona == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorInsertandoZona));
                    errorId   = (int)enumErrores.errorInsertandoZona;
                    errorDesc = errorDescBD ?? enumErrores.errorInsertandoZona.ToString();
                    return res;
                }

                res.resultado = true;
                res.guidZona  = guidZona;
                res.error     = null;
                tipoBitacora  = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId   = (int)enumErrores.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(null, tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // =====================================================================
        // OBTENER ZONAS
        // =====================================================================
        public ResObtenerZonas obtener(ReqObtenerZonas req)
        {
            ResObtenerZonas res = new ResObtenerZonas();
            res.resultado = false;
            res.error     = new List<Error>();
            res.zonas     = new List<Core.Entidades.Zona>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                List<SP_OBTENER_ZONASResult> lista;
                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    lista = linq.SP_OBTENER_ZONAS().ToList();
                }

                res.zonas     = factoriaZonas(lista);
                res.resultado = true;
                res.error     = null;
                tipoBitacora  = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId   = (int)enumErrores.errorNoControlado;
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
        private List<Core.Entidades.Zona> factoriaZonas(List<SP_OBTENER_ZONASResult> lista)
        {
            List<Core.Entidades.Zona> resultado = new List<Core.Entidades.Zona>();
            foreach (var sp in lista)
            {
                resultado.Add(new Core.Entidades.Zona
                {
                    guidZona      = sp.GUID_ZONA,
                    nombre        = sp.NOMBRE,
                    descripcion   = sp.DESCRIPCION,
                    estado        = sp.ESTADO,
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
                    clase       = GetType().Name,
                    metodo      = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name,
                    tipo        = tipo,
                    errorId     = errorId,
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
