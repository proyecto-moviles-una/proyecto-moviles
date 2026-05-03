using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using API.Filters;
using DTO.Usuario;
using Logica.Usuario;
using System;
using System.Web.Http;

namespace API.Controllers
{
    public class AuthController : ApiController
    {
        // POST api/auth/registro  � p�blico
        [HttpPost]
        [Route("api/auth/registro")]
        public IHttpActionResult registro(DTOInsertarUsuario dtoUsuario)
        {
            try
            {
                ReqInsertarUsuario req = new ReqInsertarUsuario();
                req.usuario = new Usuario();
                req.usuario.nombre    = dtoUsuario.nombre;
                req.usuario.apellidos = dtoUsuario.apellidos;
                req.usuario.email     = dtoUsuario.email;
                req.usuario.password  = dtoUsuario.password;

                return Ok(new LogUsuario().registrar(req));
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    inner2 = ex.InnerException?.InnerException?.Message
                });
            }
        }

        // POST api/auth/activar
        [HttpPost]
        [Route("api/auth/activar")]
        public ResActivarUsuario activar(DTOActivarUsuario dtoActivar)
        {
            ReqActivarUsuario req = new ReqActivarUsuario();
            req.correo = dtoActivar.correo;
            req.token  = dtoActivar.token;

            return new LogUsuario().activar(req);
        }

        // POST api/auth/login
        [HttpPost]
        [Route("api/auth/login")]
        public ResLogin login(DTOLogin dtoLogin)
        {
            ReqLogin req = new ReqLogin();
            req.email    = dtoLogin.email;
            req.password = dtoLogin.password;

            return new LogUsuario().login(req);
        }

        // POST api/auth/logout  � requiere JWT
        [HttpPost]
        [JwtAuth]
        [Route("api/auth/logout")]
        public ResCerrarSesion logout(DTOCerrarSesion dtoCerrar)
        {
            ReqCerrarSesion req = new ReqCerrarSesion();
            req.guidSesion = dtoCerrar.guidSesion;

            return new LogUsuario().logout(req);
        }
    }
}
