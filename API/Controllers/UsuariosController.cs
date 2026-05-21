using Core.Entidades.Request;
using Core.Entidades.Request;
using Core.Entidades.Response;
using API.Filters;
using DTO.Usuario;
using Logica.Auth;
using Logica.Usuario;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    [JwtAuth]
    public class UsuariosController : ApiController
    {
        // Extrae el TokenInfo inyectado por JwtAuthFilter 
        private TokenInfo tokenActual =>
            Request.Properties.ContainsKey("tokenInfo")
                ? (TokenInfo)Request.Properties["tokenInfo"]
                : null;

        // GET api/usuarios/listar  — cualquier usuario autenticado puede listar
        // GET api/usuarios/listar  — solo admin puede ver la lista completa
        [HttpGet]
        [Route("api/usuarios/listar")]
        public HttpResponseMessage listar()
        {
            if (tokenActual == null || tokenActual.rol != "admin")
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Acceso denegado. Se requiere rol admin.");

            return Request.CreateResponse(HttpStatusCode.OK, new LogUsuario().obtenerLista(new ReqObtenerListaUsuarios()));
        }

        // GET api/usuarios/perfil  — obtiene el perfil del usuario autenticado (guid viene del JWT)
        [HttpGet]
        [Route("api/usuarios/perfil")]
        public HttpResponseMessage perfil()
        {
            Guid guid = Guid.Parse(tokenActual.guidUsuario);
            ReqObtenerUsuario req = new ReqObtenerUsuario();
            req.guid = guid;
            return Request.CreateResponse(HttpStatusCode.OK, new LogUsuario().obtenerPerfil(req));
        }

        // PUT api/usuarios/perfil  — actualiza el perfil del usuario autenticado (guid viene del JWT)
        [HttpPut]
        [Route("api/usuarios/perfil")]
        public HttpResponseMessage actualizar(DTOActualizarUsuario dtoActualizar)
        {
            Guid guid = Guid.Parse(tokenActual.guidUsuario);
            ReqActualizarUsuario req = new ReqActualizarUsuario();
            req.guidUsuario = guid;
            req.nombre      = dtoActualizar.nombre;
            req.apellidos   = dtoActualizar.apellidos;
            return Request.CreateResponse(HttpStatusCode.OK, new LogUsuario().actualizarPerfil(req));
        }

        // DELETE api/usuarios  — elimina la cuenta del usuario autenticado (guid viene del JWT)
        [HttpDelete]
        [Route("api/usuarios")]
        public HttpResponseMessage eliminar()
        {
            Guid guid = Guid.Parse(tokenActual.guidUsuario);
            ReqEliminarUsuario req = new ReqEliminarUsuario();
            req.guidUsuario = guid;
            return Request.CreateResponse(HttpStatusCode.OK, new LogUsuario().eliminar(req));
        }

        // PUT api/usuarios/desactivar  — desactiva la cuenta del usuario autenticado (guid viene del JWT)
        [HttpPut]
        [Route("api/usuarios/desactivar")]
        public HttpResponseMessage desactivar()
        {
            Guid guid = Guid.Parse(tokenActual.guidUsuario);
            ReqDesactivarUsuario req = new ReqDesactivarUsuario();
            req.guidUsuario = guid;
            return Request.CreateResponse(HttpStatusCode.OK, new LogUsuario().desactivar(req));
        }

        // PUT api/usuarios/perfil/password  — cambia la contraseña del usuario autenticado (guid viene del JWT)
        [HttpPut]
        [Route("api/usuarios/perfil/password")]
        public HttpResponseMessage cambiarPassword(DTOCambiarPassword dto)
        {
            Guid guid = Guid.Parse(tokenActual.guidUsuario);
            ReqCambiarPassword req = new ReqCambiarPassword();
            req.guidUsuario        = guid;
            req.passwordActual     = dto.passwordActual;
            req.passwordNueva      = dto.passwordNueva;
            req.confirmarPassword  = dto.confirmarPassword;
            return Request.CreateResponse(HttpStatusCode.OK, new LogUsuario().cambiarPassword(req));
        }

        // POST api/usuarios/perfil/correo/solicitar  — Paso 1: solicita cambio de correo (guid viene del JWT)
        [HttpPost]
        [Route("api/usuarios/perfil/correo/solicitar")]
        public HttpResponseMessage solicitarCambioCorreo(DTOSolicitarCambioCorreo dto)
        {
            Guid guid = Guid.Parse(tokenActual.guidUsuario);
            ReqSolicitarCambioCorreo req = new ReqSolicitarCambioCorreo();
            req.guidUsuario    = guid;
            req.passwordActual = dto.passwordActual;
            req.correoNuevo    = dto.correoNuevo;
            return Request.CreateResponse(HttpStatusCode.OK, new LogUsuario().solicitarCambioCorreo(req));
        }

        // POST api/usuarios/perfil/correo/confirmar  — Paso 2: confirma el cambio de correo (guid viene del JWT)
        [HttpPost]
        [Route("api/usuarios/perfil/correo/confirmar")]
        public HttpResponseMessage confirmarCambioCorreo(DTOConfirmarCambioCorreo dto)
        {
            Guid guid = Guid.Parse(tokenActual.guidUsuario);
            ReqConfirmarCambioCorreo req = new ReqConfirmarCambioCorreo();
            req.guidUsuario = guid;
            req.codigo      = dto.codigo;
            return Request.CreateResponse(HttpStatusCode.OK, new LogUsuario().confirmarCambioCorreo(req));
        }
    }
}

