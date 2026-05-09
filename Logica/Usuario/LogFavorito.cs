using AccesoDatos;
using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum;
using Core.Enum.Autenticacion;
using Core.Enum.Favoritos;
using Core.Enum.Generales;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Usuario
{
    public class LogFavorito
    {
        // ?????????????????????????????????????????????????????????????????????
        // AGREGAR FAVORITO
        // ?????????????????????????????????????????????????????????????????????
        public ResAgregarFavorito agregar(ReqAgregarFavorito req)
        {
            ResAgregarFavorito res = new ResAgregarFavorito();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (req.guidUsuario == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.guidDeUsuarioFaltante));
                    return res;
                }
                if (req.guidRuta == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresFavoritos.guidRutaFaltante));
                    return res;
                }

                System.Nullable<System.Guid> guidFavorito = null;
                System.Nullable<int>         idReturn     = null;
                System.Nullable<int>         errorIdBD    = null;
                string                       errorDescBD  = null;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    linq.SP_AGREGAR_FAVORITO(
                        req.guidUsuario,
                        req.guidRuta,
                        ref guidFavorito,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (guidFavorito == null || guidFavorito == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorBaseDatos));
                    errorId   = (int)enumErroresGenerales.errorBaseDatos;
                    errorDesc = enumErroresGenerales.errorBaseDatos.ToString();
                    return res;
                }

                res.resultado    = true;
                res.guidFavorito = guidFavorito;
                res.error        = null;
                tipoBitacora     = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorNoControlado));
                errorId   = (int)enumErroresGenerales.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // ?????????????????????????????????????????????????????????????????????
        // ELIMINAR FAVORITO
        // ?????????????????????????????????????????????????????????????????????
        public ResEliminarFavorito eliminar(ReqEliminarFavorito req)
        {
            ResEliminarFavorito res = new ResEliminarFavorito();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (req.guidFavorito == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresFavoritos.guidFavoritoFaltante));
                    return res;
                }

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    linq.SP_ELIMINAR_FAVORITO(
                        req.guidFavorito,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (idReturn > 0)
                {
                    res.resultado = true;
                    res.error     = null;
                    tipoBitacora  = enumBitacora.exitoso;
                }
                else
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresFavoritos.favoritoNoExiste));
                    errorId   = (int)enumErroresFavoritos.favoritoNoExiste;
                    errorDesc = enumErroresFavoritos.favoritoNoExiste.ToString();
                }
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorNoControlado));
                errorId   = (int)enumErroresGenerales.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // ?????????????????????????????????????????????????????????????????????
        // OBTENER FAVORITOS POR USUARIO
        // ?????????????????????????????????????????????????????????????????????
        public ResObtenerFavoritos obtener(ReqObtenerFavoritos req)
        {
            ResObtenerFavoritos res = new ResObtenerFavoritos();
            res.resultado  = false;
            res.error      = new List<Error>();
            res.favoritos  = new List<Favorito>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (req.guidUsuario == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.guidDeUsuarioFaltante));
                    return res;
                }

                List<SP_OBTENER_FAVORITOS_POR_USUARIOResult> lista;
                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    lista = linq.SP_OBTENER_FAVORITOS_POR_USUARIO(req.guidUsuario).ToList();
                }

                res.favoritos = factoriaFavoritos(lista);
                res.resultado = true;
                res.error     = null;
                tipoBitacora  = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorNoControlado));
                errorId   = (int)enumErroresGenerales.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // ?????????????????????????????????????????????????????????????????????
        // FACTORÍA
        // ?????????????????????????????????????????????????????????????????????
        private List<Favorito> factoriaFavoritos(List<SP_OBTENER_FAVORITOS_POR_USUARIOResult> lista)
        {
            List<Favorito> resultado = new List<Favorito>();
            foreach (var sp in lista)
            {
                resultado.Add(new Favorito
                {
                    guidFavorito  = sp.GUID_FAVORITO,
                    guidRuta      = sp.GUID_RUTA,
                    nombreRuta    = sp.NOMBRE_RUTA,
                    origen        = sp.ORIGEN,
                    destino       = sp.DESTINO,
                    tarifaActual  = sp.TARIFA_ACTUAL,
                    fechaRegistro = sp.FECHA_REGISTRO
                });
            }
            return resultado;
        }

        // ?????????????????????????????????????????????????????????????????????
        // BITÁCORA
        // ?????????????????????????????????????????????????????????????????????
        private void bitacorear(enumBitacora tipo, int errorId,
                                string errorDesc, object req, object res)
        {
            try
            {
                string dispositivo = System.Web.HttpContext.Current?.Request?.UserAgent ?? "desconocido";

                ReqBitacorear reqBit = new ReqBitacorear();
                reqBit.bitacora = new Bitacora
                {
                    dispositivo = dispositivo,
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
