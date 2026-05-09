using Core.Entidades;
using System;
using Core.Entidades.Request;
using Core.Entidades.Response;
using API.Filters;
using DTO.Usuario;
using Logica.Usuario;
using System.Web.Http;

namespace API.Controllers
{
    public class AuthController : ApiController
    {
        // POST api/auth/registro  — público
        [HttpPost]
        [Route("api/auth/registro")]
        public ResInsertarUsuario registro(DTOInsertarUsuario dtoUsuario)
        {
            ReqInsertarUsuario req = new ReqInsertarUsuario();
            req.usuario = new Usuario();
            req.usuario.nombre    = dtoUsuario.nombre;
            req.usuario.apellidos = dtoUsuario.apellidos;
            req.usuario.email     = dtoUsuario.email;
            req.usuario.password  = dtoUsuario.password;

            return new LogUsuario().registrar(req);
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

        // POST api/auth/logout  — requiere JWT
        [HttpPost]
        [JwtAuth]
        [Route("api/auth/logout")]
        public ResCerrarSesion logout()
        {
            Logica.Auth.TokenInfo tokenInfo = (Logica.Auth.TokenInfo)Request.Properties["tokenInfo"];

            ReqCerrarSesion req = new ReqCerrarSesion();
            req.guidSesion = Guid.Parse(tokenInfo.guidSesion);

            return new LogUsuario().logout(req);
        }

        // POST api/auth/reenviar-activacion  — público (el usuario aún no puede loguearse)
        [HttpPost]
        [Route("api/auth/reenviar-activacion")]
        public ResReenviarActivacion reenviarActivacion(DTOReenviarActivacion dto)
        {
            ReqReenviarActivacion req = new ReqReenviarActivacion();
            req.correo = dto.correo;

            return new LogUsuario().reenviarActivacion(req);
        }

        // POST api/auth/solicitar-reactivacion  — público (la cuenta está desactivada, no puede hacer login)
        // Genera un código y lo envía al correo del usuario desactivado
        [HttpPost]
        [Route("api/auth/solicitar-reactivacion")]
        public ResSolicitarReactivacion solicitarReactivacion(DTOSolicitarReactivacion dto)
        {
            ReqSolicitarReactivacion req = new ReqSolicitarReactivacion();
            req.correo = dto.email;

            return new LogUsuario().solicitarReactivacion(req);
        }

        // POST api/auth/reactivar  — público
        // Valida el código y reactiva la cuenta (ESTADO 2 ? 1)
        [HttpPost]
        [Route("api/auth/reactivar")]
        public ResReactivarUsuario reactivar(DTOReactivarUsuario dto)
        {
            ReqReactivarUsuario req = new ReqReactivarUsuario();
            req.correo = dto.email;
            req.token  = dto.token;

            return new LogUsuario().reactivar(req);
        }
    }
}
