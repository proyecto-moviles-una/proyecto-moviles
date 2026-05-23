using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum;
using Core.Enum.Autenticacion;
using Core.Enum.Generales;
using Core.Enum.Perfil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.nombreFaltante));
                    errorId   = (int)enumErroresAutenticacion.nombreFaltante;
                    errorDesc = enumErroresAutenticacion.nombreFaltante.ToString();
                }

                if (string.IsNullOrEmpty(req.usuario.apellidos))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.apellidosFaltante));
                    errorId   = (int)enumErroresAutenticacion.apellidosFaltante;
                    errorDesc = enumErroresAutenticacion.apellidosFaltante.ToString();
                }

                if (string.IsNullOrEmpty(req.usuario.email))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.emailFaltante));
                    errorId   = (int)enumErroresAutenticacion.emailFaltante;
                    errorDesc = enumErroresAutenticacion.emailFaltante.ToString();
                }
                else if (!Utilitarios.Utilitarios.EsEmailValido(req.usuario.email))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.emailInvalido));
                    errorId   = (int)enumErroresAutenticacion.emailInvalido;
                    errorDesc = enumErroresAutenticacion.emailInvalido.ToString();
                }

                if (string.IsNullOrEmpty(req.usuario.password))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.passwordVacio));
                    errorId   = (int)enumErroresAutenticacion.passwordVacio;
                    errorDesc = enumErroresAutenticacion.passwordVacio.ToString();
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

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
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
                    // Si el SP devolvió errorId=1 es correo duplicado, si no es error genérico de BD
                    int codError = (errorIdBD == 1)
                        ? (int)enumErroresAutenticacion.correoYaRegistrado
                        : (int)enumErroresGenerales.errorBaseDatos;
                    string codErrorDesc = (errorIdBD == 1)
                        ? enumErroresAutenticacion.correoYaRegistrado.ToString()
                        : enumErroresGenerales.errorBaseDatos.ToString();

                    if (errorIdBD != 1)
                        Utilitarios.Utilitarios.registrarErrorBD("SP_INGRESAR_USUARIO", errorIdBD ?? 0, errorDescBD);

                    res.error.Add(Utilitarios.Utilitarios.crearError(codError));
                    errorId   = codError;
                    errorDesc = errorDescBD ?? codErrorDesc;
                    return res;
                }

                // ?? Enviar correo de verificación ??????????????????????????
                bool correoEnviado = Utilitarios.Utilitarios.EnviarCorreoVerificacion(
                    req.usuario.nombre, req.usuario.apellidos, req.usuario.email, token);

                res.resultado    = true;
                res.mensaje      = "Usuario registrado correctamente. Revise su correo para activar la cuenta.";
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

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
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
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.errorActivandoUsuario));
                    errorId   = (int)enumErroresAutenticacion.errorActivandoUsuario;
                    errorDesc = enumErroresAutenticacion.errorActivandoUsuario.ToString();
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
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.emailFaltante));
                    return res;
                }
                if (string.IsNullOrEmpty(req.password))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.passwordVacio));
                    return res;
                }

                // ?? Buscar usuario por correo ??????????????????????????????
                SP_LOGINResult spResult = null;
                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    spResult = linq.SP_LOGIN(req.email).FirstOrDefault();
                }

                if (spResult == null)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.loginIncorrecto));
                    errorId   = (int)enumErroresAutenticacion.loginIncorrecto;
                    errorDesc = enumErroresAutenticacion.loginIncorrecto.ToString();
                    return res;
                }

                // ?? Verificar contraseña con BCrypt ????????????????????????
                if (!Utilitarios.Utilitarios.verificarPassword(req.password, spResult.HASH_PASSWORD))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.loginIncorrecto));
                    errorId   = (int)enumErroresAutenticacion.loginIncorrecto;
                    errorDesc = enumErroresAutenticacion.loginIncorrecto.ToString();
                    return res;
                }

                // ?? Verificar que la cuenta esté activa ????????????????????
                if (spResult.ESTADO == 0)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.usuarioInactivo));
                    errorId   = (int)enumErroresAutenticacion.usuarioInactivo;
                    errorDesc = enumErroresAutenticacion.usuarioInactivo.ToString();
                    return res;
                }
                if (spResult.ESTADO != 1)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.usuarioDesactivado));
                    errorId   = (int)enumErroresAutenticacion.usuarioDesactivado;
                    errorDesc = enumErroresAutenticacion.usuarioDesactivado.ToString();
                    return res;
                }

                // ?? Abrir sesión en BD ?????????????????????????????????????
                string                       jwtTemp     = Utilitarios.Utilitarios.crearToken();
                System.Nullable<System.Guid> guidSesion  = null;
                System.Nullable<int>         idReturn    = null;
                System.Nullable<int>         errorIdBD   = null;
                string                       errorDescBD = null;

                string origenSesion = null;
                System.Web.HttpContext ctxLogin = System.Web.HttpContext.Current;
                if (ctxLogin != null)
                    origenSesion = ctxLogin.Request.UserAgent;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    linq.SP_ABRIR_SESION(
                        jwtTemp,
                        spResult.GUID_USUARIO,
                        origenSesion,
                        ref guidSesion,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (guidSesion == null || guidSesion == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.errorAbrirSesion));
                    errorId   = (int)enumErroresAutenticacion.errorAbrirSesion;
                    errorDesc = enumErroresAutenticacion.errorAbrirSesion.ToString();
                    return res; // si el guid n se genero viene vacion entonces nodeja 
                }
                /// s ya se creo el gid  entonces se inserta el guid en el JWt
                // ?? Generar JWT final con guidSesion real ??????????????????
                string jwt = Utilitarios.Utilitarios.generarJWT(
                    spResult.GUID_USUARIO,
                    guidSesion.Value,
                    spResult.NOMBRE,
                    spResult.ROL ?? "usuario");

                res.resultado  = true;
                res.error      = null;
                res.guidSesion = guidSesion;
                res.usuario    = new Core.Entidades.Usuario
                {
                    guid      = spResult.GUID_USUARIO,
                    nombre    = spResult.NOMBRE,
                    apellidos = spResult.APELLIDOS,
                    rol       = spResult.ROL
                };
                res.token     = jwt;
                tipoBitacora  = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorNoControlado));
#if DEBUG
                res.error.Add(new Error
                {
                    Codigo = -999,
                    Mensaje = ex.GetType().Name + ": " + ex.Message
                });
#endif
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
            //? valida que venga
            //? llama SP_CERRAR_SESION
             //? si cerró 1 sesión, éxito
            //? si no, error
             //? guarda bitácora

            try
            {
                if (req.guidSesion == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.guidSesionFaltante));
                    return res;
                }

                int filasActualizadas;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    filasActualizadas = linq.SP_CERRAR_SESION(req.guidSesion);
                }

                if (filasActualizadas == 1)
                {
                    res.resultado = true;
                    res.error     = null;
                    tipoBitacora  = enumBitacora.exitoso;
                }
                else
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.sesionYaCerrada));
                    errorId   = (int)enumErroresAutenticacion.sesionYaCerrada;
                    errorDesc = enumErroresAutenticacion.sesionYaCerrada.ToString();
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
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.guidDeUsuarioFaltante));
                    return res;
                }

                SP_OBTENER_USUARIOResult spResult = null;
                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    spResult = linq.SP_OBTENER_USUARIO(req.guid).FirstOrDefault();
                }

                if (spResult == null)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.guidDeUsuarioFaltante));
                    return res;
                }

                res.resultado = true;
                res.error     = null;
                res.usuario   = factoriaUsuario(spResult);
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
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.guidDeUsuarioFaltante));
                    return res;
                }
                if (string.IsNullOrEmpty(req.nombre))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.nombreFaltante));
                    return res;
                }
                if (string.IsNullOrEmpty(req.apellidos))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.apellidosFaltante));
                    return res;
                }

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
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
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorBaseDatos));
                    errorId   = (int)enumErroresGenerales.errorBaseDatos;
                    errorDesc = errorDescBD ?? enumErroresGenerales.errorBaseDatos.ToString();
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
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.guidDeUsuarioFaltante));
                    return res;
                }

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
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
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.guidDeUsuarioFaltante));
                    errorId   = (int)enumErroresAutenticacion.guidDeUsuarioFaltante;
                    errorDesc = errorDescBD;
                }
                else
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorBaseDatos));
                    errorId   = (int)enumErroresGenerales.errorBaseDatos;
                    errorDesc = errorDescBD;
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
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.guidDeUsuarioFaltante));
                    return res;
                }

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
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
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.guidDeUsuarioFaltante));
                    errorId   = (int)enumErroresAutenticacion.guidDeUsuarioFaltante;
                    errorDesc = errorDescBD;
                }
                else
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorBaseDatos));
                    errorId   = (int)enumErroresGenerales.errorBaseDatos;
                    errorDesc = errorDescBD;
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
        // REENVIAR ACTIVACIÓN
        // ?????????????????????????????????????????????????????????????????????
        public ResReenviarActivacion reenviarActivacion(ReqReenviarActivacion req)
        {
            ResReenviarActivacion res = new ResReenviarActivacion();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(req.correo))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.emailFaltante));
                    return res;
                }

                string token = Utilitarios.Utilitarios.crearToken();

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    linq.SP_REENVIAR_ACTIVACION(
                        req.correo,
                        token,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (idReturn == null || idReturn <= 0)
                {
                    Utilitarios.Utilitarios.registrarErrorBD("SP_REENVIAR_ACTIVACION", errorIdBD ?? 0, errorDescBD);
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorBaseDatos));
                    errorId   = (int)enumErroresGenerales.errorBaseDatos;
                    errorDesc = errorDescBD ?? enumErroresGenerales.errorBaseDatos.ToString();
                    return res;
                }

                // Obtener nombre del usuario para el correo no es posible sin SP adicional,
                // se envía solo con correo
                bool correoEnviado = Utilitarios.Utilitarios.EnviarCorreoVerificacion(
                    string.Empty, string.Empty, req.correo, token);

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
        // SOLICITAR CAMBIO DE CORREO
        // ?????????????????????????????????????????????????????????????????????
        public ResSolicitarCambioCorreo solicitarCambioCorreo(ReqSolicitarCambioCorreo req)
        {
            ResSolicitarCambioCorreo res = new ResSolicitarCambioCorreo();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                // Validaciones
                if (string.IsNullOrEmpty(req.passwordActual))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresPerfil.passwordActualIncorrecto));
                    return res;
                }
                if (string.IsNullOrEmpty(req.correoNuevo))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresPerfil.correoNuevoFaltante));
                    return res;
                }
                if (!Utilitarios.Utilitarios.EsEmailValido(req.correoNuevo))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.emailInvalido));
                    return res;
                }

                // Verificar la contraseña actual del usuario
                SP_LOGINResult usuarioBD = null;
                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    usuarioBD = linq.SP_OBTENER_HASH_USUARIO(req.guidUsuario).FirstOrDefault();
                }

                if (usuarioBD == null)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.guidDeUsuarioFaltante));
                    errorId   = (int)enumErroresAutenticacion.guidDeUsuarioFaltante;
                    errorDesc = enumErroresAutenticacion.guidDeUsuarioFaltante.ToString();
                    return res;
                }

                if (!Utilitarios.Utilitarios.verificarPassword(req.passwordActual, usuarioBD.HASH_PASSWORD))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresPerfil.passwordActualIncorrecto));
                    errorId   = (int)enumErroresPerfil.passwordActualIncorrecto;
                    errorDesc = enumErroresPerfil.passwordActualIncorrecto.ToString();
                    return res;
                }

                // Generar código y guardar correo pendiente en BD
                string codigo = Utilitarios.Utilitarios.crearToken();

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    linq.SP_SOLICITAR_CAMBIO_CORREO(
                        req.guidUsuario,
                        req.correoNuevo,
                        codigo,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (errorIdBD == 1)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.correoYaRegistrado));
                    errorId   = (int)enumErroresAutenticacion.correoYaRegistrado;
                    errorDesc = errorDescBD;
                    return res;
                }

                if (idReturn == null || idReturn <= 0)
                {
                    Utilitarios.Utilitarios.registrarErrorBD("SP_SOLICITAR_CAMBIO_CORREO", errorIdBD ?? 0, errorDescBD);
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorBaseDatos));
                    errorId   = (int)enumErroresGenerales.errorBaseDatos;
                    errorDesc = errorDescBD ?? enumErroresGenerales.errorBaseDatos.ToString();
                    return res;
                }

                // Enviar código al nuevo correo
                Utilitarios.Utilitarios.EnviarCodigoCambioCorreo(
                    usuarioBD.NOMBRE, usuarioBD.APELLIDOS, req.correoNuevo, codigo);

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
        // CONFIRMAR CAMBIO DE CORREO
        // ?????????????????????????????????????????????????????????????????????
        public ResConfirmarCambioCorreo confirmarCambioCorreo(ReqConfirmarCambioCorreo req)
        {
            ResConfirmarCambioCorreo res = new ResConfirmarCambioCorreo();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(req.codigo))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresPerfil.codigoVerificacionFaltante));
                    return res;
                }

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    linq.SP_CONFIRMAR_CAMBIO_CORREO(
                        req.guidUsuario,
                        req.codigo.ToUpper(),
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
                else if (errorIdBD == 12)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.codigoExpirado));
                    errorId   = (int)enumErroresAutenticacion.codigoExpirado;
                    errorDesc = errorDescBD;
                }
                else if (errorIdBD == 43)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresPerfil.codigoVerificacionInvalido));
                    errorId   = (int)enumErroresPerfil.codigoVerificacionInvalido;
                    errorDesc = errorDescBD;
                }
                else
                {
                    Utilitarios.Utilitarios.registrarErrorBD("SP_CONFIRMAR_CAMBIO_CORREO", errorIdBD ?? 0, errorDescBD);
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorBaseDatos));
                    errorId   = (int)enumErroresGenerales.errorBaseDatos;
                    errorDesc = errorDescBD ?? enumErroresGenerales.errorBaseDatos.ToString();
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
        // SOLICITAR REACTIVACIÓN
        // ?????????????????????????????????????????????????????????????????????
        public ResSolicitarReactivacion solicitarReactivacion(ReqSolicitarReactivacion req)
        {
            ResSolicitarReactivacion res = new ResSolicitarReactivacion();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(req.correo))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.emailFaltante));
                    return res;
                }

                string codigo = Utilitarios.Utilitarios.crearToken();

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;
                string               nombre      = null;
                string               apellidos   = null;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    linq.SP_SOLICITAR_REACTIVACION(
                        req.correo,
                        codigo,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD,
                        ref nombre,
                        ref apellidos);
                }

                if (errorIdBD == 14)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.correoNoRegistrado));
                    errorId   = (int)enumErroresAutenticacion.correoNoRegistrado;
                    errorDesc = errorDescBD;
                    return res;
                }

                if (errorIdBD == 47)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.usuarioNoDesactivado));
                    errorId   = (int)enumErroresAutenticacion.usuarioNoDesactivado;
                    errorDesc = errorDescBD;
                    return res;
                }

                if (idReturn == null || idReturn <= 0)
                {
                    Utilitarios.Utilitarios.registrarErrorBD("SP_SOLICITAR_REACTIVACION", errorIdBD ?? 0, errorDescBD);
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorBaseDatos));
                    errorId   = (int)enumErroresGenerales.errorBaseDatos;
                    errorDesc = errorDescBD ?? enumErroresGenerales.errorBaseDatos.ToString();
                    return res;
                }

                // Enviar código al correo del usuario
                Utilitarios.Utilitarios.EnviarCodigoReactivacion(
                    nombre ?? string.Empty, apellidos ?? string.Empty, req.correo, codigo);

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
        // CONFIRMAR REACTIVACIÓN
        // ?????????????????????????????????????????????????????????????????????
        public ResReactivarUsuario reactivar(ReqReactivarUsuario req)
        {
            ResReactivarUsuario res = new ResReactivarUsuario();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(req.correo))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.emailFaltante));
                    return res;
                }
                if (string.IsNullOrEmpty(req.token))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresPerfil.codigoVerificacionFaltante));
                    return res;
                }

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    linq.SP_REACTIVAR_USUARIO(
                        req.correo,
                        req.token.ToUpper(),
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
                else if (errorIdBD == 12)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.codigoExpirado));
                    errorId   = (int)enumErroresAutenticacion.codigoExpirado;
                    errorDesc = errorDescBD;
                }
                else if (errorIdBD == 45)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresPerfil.codigoVerificacionInvalido));
                    errorId   = (int)enumErroresPerfil.codigoVerificacionInvalido;
                    errorDesc = errorDescBD;
                }
                else if (errorIdBD == 47)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.usuarioNoDesactivado));
                    errorId   = (int)enumErroresAutenticacion.usuarioNoDesactivado;
                    errorDesc = errorDescBD;
                }
                else
                {
                    Utilitarios.Utilitarios.registrarErrorBD("SP_REACTIVAR_USUARIO", errorIdBD ?? 0, errorDescBD);
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorBaseDatos));
                    errorId   = (int)enumErroresGenerales.errorBaseDatos;
                    errorDesc = errorDescBD ?? enumErroresGenerales.errorBaseDatos.ToString();
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
                estado    = sp.ESTADO,
                rol       = sp.ROL
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
                    estado    = item.ESTADO,
                    rol       = item.ROL
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
                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
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
        // BITÁCORA (privado — igual que el profe en Finally)
        // ?????????????????????????????????????????????????????????????????????
        private void bitacorear(enumBitacora tipo, int errorId,
                                string errorDesc, object req, object res,
                                [System.Runtime.CompilerServices.CallerMemberName] string metodo = "")
        {
            try
            {
                ReqBitacorear reqBit = new ReqBitacorear();
                reqBit.bitacora = new Bitacora
                {
                    clase       = GetType().Name,
                    metodo      = metodo,
                    tipo        = tipo,
                    codigoError = errorId,
                    descripcion = errorDesc,
                    request     = JsonConvert.SerializeObject(req),
                    response    = JsonConvert.SerializeObject(res)
                };
                Utilitarios.Utilitarios.bitacorear(reqBit);
            }
            catch { }
        }

        // ?????????????????????????????????????????????????????????????????????
        // CAMBIAR CONTRASEÑA
        // ?????????????????????????????????????????????????????????????????????
        public ResCambiarPassword cambiarPassword(ReqCambiarPassword req)
        {
            ResCambiarPassword res = new ResCambiarPassword();
            res.resultado = false;
            res.error     = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int          errorId      = 0;
            string       errorDesc    = string.Empty;

            try
            {
                // Validaciones
                if (string.IsNullOrEmpty(req.passwordActual))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresPerfil.passwordActualIncorrecto));
                    return res;
                }

                if (string.IsNullOrEmpty(req.passwordNueva))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresPerfil.passwordNuevoVacio));
                    return res;
                }

                if (req.passwordNueva != req.confirmarPassword)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresPerfil.passwordsNoCoinciden));
                    return res;
                }

                // Obtener el hash actual del usuario por su GUID
                SP_LOGINResult usuarioBD = null;
                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    usuarioBD = linq.SP_OBTENER_HASH_USUARIO(req.guidUsuario).FirstOrDefault();
                }

                if (usuarioBD == null)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresAutenticacion.guidDeUsuarioFaltante));
                    errorId   = (int)enumErroresAutenticacion.guidDeUsuarioFaltante;
                    errorDesc = enumErroresAutenticacion.guidDeUsuarioFaltante.ToString();
                    return res;
                }

                // Verificar que la contraseña actual coincide con el hash guardado
                if (!Utilitarios.Utilitarios.verificarPassword(req.passwordActual, usuarioBD.HASH_PASSWORD))
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresPerfil.passwordActualIncorrecto));
                    errorId   = (int)enumErroresPerfil.passwordActualIncorrecto;
                    errorDesc = enumErroresPerfil.passwordActualIncorrecto.ToString();
                    return res;
                }

                // Hashear la nueva contraseña
                string nuevoHash = Utilitarios.Utilitarios.hashPassword(req.passwordNueva);

                System.Nullable<int> idReturn    = null;
                System.Nullable<int> errorIdBD   = null;
                string               errorDescBD = null;

                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    linq.SP_CAMBIAR_PASSWORD(req.guidUsuario, nuevoHash, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (idReturn > 0)
                {
                    res.resultado = true;
                    res.error     = null;
                    tipoBitacora  = enumBitacora.exitoso;
                }
                else
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError((int)enumErroresGenerales.errorBaseDatos));
                    errorId   = (int)enumErroresGenerales.errorBaseDatos;
                    errorDesc = errorDescBD ?? enumErroresGenerales.errorBaseDatos.ToString();
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
    }
}
