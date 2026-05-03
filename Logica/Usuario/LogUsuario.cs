using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Logica.Usuario
{
    public class LogUsuario
    {
        // ?????????????????????????????????????????????????????????????????????
        // REGISTRAR
        // ?????????????????????????????????????????????????????????????????????
        public ResInsertarUsuario registrar(ReqInsertarUsuario req)
        {
            ResInsertarUsuario res = new ResInsertarUsuario();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora  = enumBitacora.fallido;
            int          errorId       = 0;
            string       errorDesc     = string.Empty;

            try
            {
                // ?? Validaciones ??????????????????????????????????????????
                if (string.IsNullOrEmpty(req.usuario.nombre))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.nombreFaltante));
                    errorId   = (int)enumErrores.nombreFaltante;
                    errorDesc = enumErrores.nombreFaltante.ToString();
                }

                if (string.IsNullOrEmpty(req.usuario.apellidos))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.apellidosFaltante));
                    errorId   = (int)enumErrores.apellidosFaltante;
                    errorDesc = enumErrores.apellidosFaltante.ToString();
                }

                if (string.IsNullOrEmpty(req.usuario.email))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.emailFaltante));
                    errorId   = (int)enumErrores.emailFaltante;
                    errorDesc = enumErrores.emailFaltante.ToString();
                }
                else if (!Utilitarios.Utilitarios.EsEmailValido(req.usuario.email))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.emailInvalido));
                    errorId   = (int)enumErrores.emailInvalido;
                    errorDesc = enumErrores.emailInvalido.ToString();
                }

                if (string.IsNullOrEmpty(req.usuario.password))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.passwordVacio));
                    errorId   = (int)enumErrores.passwordVacio;
                    errorDesc = enumErrores.passwordVacio.ToString();
                }

                if (res.error.Any()) return res; // si ay errores no sigo devuelvo la respues con los errores

                // ?? Hashear contraseña con BCrypt ??????????????????????????
                string hashPassword = Utilitarios.Utilitarios.hashPassword(req.usuario.password);

                // ?? Token de verificación de correo ????????????????????????
                string token = Utilitarios.Utilitarios.crearToken();

                // ?? Llamar SP ??????????????????????????????????????????????
                System.Nullable<System.Guid> guidReturn  = null;
                System.Nullable<int>         idReturn    = null;
                System.Nullable<int>         errorIdBD   = null;
                string                       errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_INGRESAR_USUARIO(
                        req.usuario.nombre,
                        req.usuario.apellidos,
                        req.usuario.email,
                        hashPassword,
                        token,
                        ref guidReturn,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                } // aqui se guarda el usario en Bd

                if (guidReturn == null || guidReturn == Guid.Empty) // si el sp no devuleve el guide significa que fallo
                {
                    // Si el SP devolvió errorId=1 es correo duplicado, si no es error genérico
                    enumErrores codError = (errorIdBD == 1)
                        ? enumErrores.correoYaRegistrado
                        : enumErrores.errorBaseDatos;

                    res.error.Add(Utilitarios.Utilitarios.crearError(codError));
                    errorId   = (int)codError;
                    errorDesc = errorDescBD ?? codError.ToString();
                    return res;
                }

                // ?? Enviar correo de verificación ??????????????????????????
                bool correoEnviado = Utilitarios.Utilitarios.EnviarCorreoVerificacion(
                    req.usuario.nombre, req.usuario.apellidos, req.usuario.email, token);

                res.resultado         = true;
                res.guidUsuario       = guidReturn;
                res.tokenVerificacion = token;
                res.error             = null;
                tipoBitacora          = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId   = (int)enumErrores.errorNoControlado;
                errorDesc = ex.Message + " | " + ex.InnerException?.Message;
                throw;
            }
            finally
            {
                bitacorear(null, tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // ?????????????????????????????????????????????????????????????????????
        // ACTIVAR CUENTA
        // ?????????????????????????????????????????????????????????????????????
        public ResActivarUsuario activar(ReqActivarUsuario req)
        {
            ResActivarUsuario res = new ResActivarUsuario();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                System.Nullable<int> filas      = null;
                System.Nullable<int> idReturn   = null;
                System.Nullable<int> errorIdBD  = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_ACTIVAR_USUARIO(
                        req.correo,
                        req.token,
                        ref filas,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (filas == 1)
                {
                    res.resultado = true;
                    res.error     = null;
                    tipoBitacora  = enumBitacora.exitoso;
                }
                else
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorActivandoUsuario));
                    errorId   = (int)enumErrores.errorActivandoUsuario;
                    errorDesc = enumErrores.errorActivandoUsuario.ToString();
                }
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

        // ?????????????????????????????????????????????????????????????????????
        // LOGIN
        // ?????????????????????????????????????????????????????????????????????
        public ResLogin login(ReqLogin req)
        {
            ResLogin res = new ResLogin();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                // ?? Validaciones ??????????????????????????????????????????
                if (string.IsNullOrEmpty(req.email))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.emailFaltante));
                    return res;
                }
                if (string.IsNullOrEmpty(req.password))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.passwordVacio));
                    return res;
                }

                // ?? Buscar usuario por correo ??????????????????????????????
                SP_LOGINResult spResult = null;
                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    spResult = linq.SP_LOGIN(req.email).FirstOrDefault();
                }

                if (spResult == null)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.loginIncorrecto));
                    errorId   = (int)enumErrores.loginIncorrecto;
                    errorDesc = enumErrores.loginIncorrecto.ToString();
                    return res;
                }

                // ?? Verificar contraseña con BCrypt ????????????????????????
                if (!Utilitarios.Utilitarios.verificarPassword(req.password, spResult.HASH_PASSWORD))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.loginIncorrecto));
                    errorId   = (int)enumErrores.loginIncorrecto;
                    errorDesc = enumErrores.loginIncorrecto.ToString();
                    return res;
                }

                // ?? Verificar que la cuenta esté activa ????????????????????
                if (spResult.ESTADO == 0)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.usuarioInactivo));
                    errorId   = (int)enumErrores.usuarioInactivo;
                    errorDesc = enumErrores.usuarioInactivo.ToString();
                    return res;
                }
                if (spResult.ESTADO != 1)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.usuarioDesactivado));
                    errorId   = (int)enumErrores.usuarioDesactivado;
                    errorDesc = enumErrores.usuarioDesactivado.ToString();
                    return res;
                }

                // ?? Abrir sesión en BD ?????????????????????????????????????
                string                       jwtTemp     = Utilitarios.Utilitarios.crearToken();
                System.Nullable<System.Guid> guidSesion  = null;
                System.Nullable<int>         idReturn    = null;
                System.Nullable<int>         errorIdBD   = null;
                string                       errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_ABRIR_SESION(
                        jwtTemp,
                        spResult.GUID_USUARIO,
                        "MiBus-App",
                        ref guidSesion,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (guidSesion == null || guidSesion == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorAbrirSesion));
                    errorId   = (int)enumErrores.errorAbrirSesion;
                    errorDesc = enumErrores.errorAbrirSesion.ToString();
                    return res; // si el guid n se genero viene vacion entonces nodeja 
                }
                /// s ya se creo el gid  entonces se inserta el guid en el JWt
                // ?? Generar JWT final con guidSesion real ??????????????????
                string jwt = Utilitarios.Utilitarios.generarJWT(
                    spResult.GUID_USUARIO,
                    guidSesion.Value,
                    spResult.NOMBRE);

                res.resultado  = true;
                res.error      = null;
                res.guidSesion = guidSesion;
                res.usuario    = new Core.Entidades.Usuario
                {
                    guid      = spResult.GUID_USUARIO,
                    nombre    = spResult.NOMBRE,
                    apellidos = spResult.APELLIDOS
                };
                res.token     = jwt;
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

        // ?????????????????????????????????????????????????????????????????????
        // CERRAR SESIÓN (LOGOUT)
        // ?????????????????????????????????????????????????????????????????????
        public ResCerrarSesion logout(ReqCerrarSesion req)
        {
            ResCerrarSesion res = new ResCerrarSesion();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;
            //Recibe guidSesion
            //→ valida que venga
            //→ llama SP_CERRAR_SESION
             //→ si cerró 1 sesión, éxito
            //→ si no, error
             //→ guarda bitácora

            try
            {
                if (req.guidSesion == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidSesionFaltante));
                    return res;
                }

                System.Nullable<int> filas       = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_CERRAR_SESION(req.guidSesion, ref filas, ref errorIdBD, ref errorDescBD);
                }

                if (filas == 0 || errorIdBD == 1)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.sesionYaCerrada));
                    errorId   = (int)enumErrores.sesionYaCerrada;
                    errorDesc = errorDescBD ?? enumErrores.sesionYaCerrada.ToString();
                    return res;
                }

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

        // ?????????????????????????????????????????????????????????????????????
        // OBTENER PERFIL
        // ?????????????????????????????????????????????????????????????????????
        public ResObtenerUsuario obtenerPerfil(ReqObtenerUsuario req)
        {
            ResObtenerUsuario res = new ResObtenerUsuario();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (req.guid == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidDeUsuarioFaltante));
                    return res;
                }

                SP_OBTENER_USUARIOResult spResult = null;
                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    spResult = linq.SP_OBTENER_USUARIO(req.guid).FirstOrDefault();
                }

                if (spResult == null)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidDeUsuarioFaltante));
                    return res;
                }

                res.resultado = true;
                res.error     = null;
                res.usuario   = factoriaUsuario(spResult);
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
                bitacorear(req.guid, tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // ?????????????????????????????????????????????????????????????????????
        // ACTUALIZAR PERFIL
        // ?????????????????????????????????????????????????????????????????????
        public ResActualizarUsuario actualizarPerfil(ReqActualizarUsuario req)
        {
            ResActualizarUsuario res = new ResActualizarUsuario();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (req.guidUsuario == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidDeUsuarioFaltante));
                    return res;
                }
                if (string.IsNullOrEmpty(req.nombre))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.nombreFaltante));
                    return res;
                }
                if (string.IsNullOrEmpty(req.apellidos))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.apellidosFaltante));
                    return res;
                }

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_ACTUALIZAR_USUARIO(
                        req.guidUsuario,
                        req.nombre,
                        req.apellidos,
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
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorBaseDatos));
                    errorId   = (int)enumErrores.errorBaseDatos;
                    errorDesc = errorDescBD ?? enumErrores.errorBaseDatos.ToString();
                }
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId   = (int)enumErrores.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(req.guidUsuario, tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // ?????????????????????????????????????????????????????????????????????
        // ELIMINAR USUARIO
        // ?????????????????????????????????????????????????????????????????????
        public ResEliminarUsuario eliminar(ReqEliminarUsuario req)
        {
            ResEliminarUsuario res = new ResEliminarUsuario();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (req.guidUsuario == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidDeUsuarioFaltante));
                    return res;
                }

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_ELIMINAR_USUARIO(
                        req.guidUsuario,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                // El SP retorna @ID_USUARIO (> 0) en éxito, -1 si no existe o error en BD
                if (idReturn > 0)
                {
                    res.resultado = true;
                    res.error     = null;
                    tipoBitacora  = enumBitacora.exitoso;
                }
                else if (errorIdBD == 2)
                {
                    // Usuario no encontrado en BD
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidDeUsuarioFaltante));
                    errorId   = (int)enumErrores.guidDeUsuarioFaltante;
                    errorDesc = errorDescBD;
                }
                else
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorBaseDatos));
                    errorId   = (int)enumErrores.errorBaseDatos;
                    errorDesc = errorDescBD;
                }
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId   = (int)enumErrores.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(req.guidUsuario, tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // ?????????????????????????????????????????????????????????????????????
        // DESACTIVAR USUARIO
        // ?????????????????????????????????????????????????????????????????????
        public ResDesactivarUsuario desactivar(ReqDesactivarUsuario req)
        {
            ResDesactivarUsuario res = new ResDesactivarUsuario();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (req.guidUsuario == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidDeUsuarioFaltante));
                    return res;
                }

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_DESACTIVAR_USUARIO(
                        req.guidUsuario,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                // El SP retorna @ID_USUARIO (> 0) en éxito, -1 si no existe o error en BD
                // ESTADO = 2 en BD significa cuenta desactivada (también cierra sesiones activas)
                if (idReturn > 0)
                {
                    res.resultado = true;
                    res.error     = null;
                    tipoBitacora  = enumBitacora.exitoso;
                }
                else if (errorIdBD == 2)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidDeUsuarioFaltante));
                    errorId   = (int)enumErrores.guidDeUsuarioFaltante;
                    errorDesc = errorDescBD;
                }
                else
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorBaseDatos));
                    errorId   = (int)enumErrores.errorBaseDatos;
                    errorDesc = errorDescBD;
                }
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId   = (int)enumErrores.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(req.guidUsuario, tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // ?????????????????????????????????????????????????????????????????????
        // FACTORÍA
        // ?????????????????????????????????????????????????????????????????????
        private Core.Entidades.Usuario factoriaUsuario(SP_OBTENER_USUARIOResult sp)
        {
            return new Core.Entidades.Usuario
            {
                guid      = sp.GUID_USUARIO,
                nombre    = sp.NOMBRE,
                apellidos = sp.APELLIDOS,
                email     = sp.CORREO_ELECTRONICO,
                estado    = sp.ESTADO
            };
        }

        private List<Core.Entidades.Usuario> factoriaListaUsuarios(List<SP_OBTENER_LISTAUSUARIOSResult> lista)
        {
            List<Core.Entidades.Usuario> resultado = new List<Core.Entidades.Usuario>();
            foreach (SP_OBTENER_LISTAUSUARIOSResult item in lista)
            {
                resultado.Add(new Core.Entidades.Usuario
                {
                    guid      = item.GUID_USUARIO,
                    nombre    = item.NOMBRE,
                    apellidos = item.APELLIDOS,
                    email     = item.CORREO_ELECTRONICO,
                    estado    = item.ESTADO
                });
            }
            return resultado;
        }

        // ?????????????????????????????????????????????????????????????????????
        // OBTENER LISTA
        // ?????????????????????????????????????????????????????????????????????
        public ResObtenerListaUsuarios obtenerLista(ReqObtenerListaUsuarios req)
        {
            ResObtenerListaUsuarios res = new ResObtenerListaUsuarios();
            res.usuarios  = new List<Core.Entidades.Usuario>();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                List<SP_OBTENER_LISTAUSUARIOSResult> listaResultado;
                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    listaResultado = linq.SP_OBTENER_LISTAUSUARIOS().ToList();
                }

                res.usuarios  = factoriaListaUsuarios(listaResultado);
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

        // ?????????????????????????????????????????????????????????????????????
        // BITÁCORA (privado — igual que el profe en Finally)
        // ?????????????????????????????????????????????????????????????????????
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
