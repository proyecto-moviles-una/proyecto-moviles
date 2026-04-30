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

        // Valida que el guid de la URL sea el mismo del token (el usuario solo accede a sus datos)
        private HttpResponseMessage validarOwnership(Guid guid)
        {
            if (tokenActual == null || tokenActual.guidUsuario != guid.ToString())
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                    "No tiene permiso para acceder a este recurso.");
            return null;
        }

        // GET api/usuarios/listar  — cualquier usuario autenticado puede listar// aqui debo de definir un rol que sea el que vea todo mas esto 
        [HttpGet]
        [Route("api/usuarios/listar")]
        public ResObtenerListaUsuarios listar()
        {
            return new LogUsuario().obtenerLista(new ReqObtenerListaUsuarios());
        }

        // GET api/usuarios/perfil/{guid}  — solo el propio usuario
        [HttpGet]
        [Route("api/usuarios/perfil/{guid}")]
        public HttpResponseMessage perfil(Guid guid)
        {
            HttpResponseMessage bloqueo = validarOwnership(guid);
            if (bloqueo != null) return bloqueo;

            ReqObtenerUsuario req = new ReqObtenerUsuario();
            req.guid = guid;
            return Request.CreateResponse(HttpStatusCode.OK, new LogUsuario().obtenerPerfil(req));
        }

        // PUT api/usuarios/perfil/{guid}  — solo el propio usuario
        [HttpPut]
        [Route("api/usuarios/perfil/{guid}")]
        public HttpResponseMessage actualizar(Guid guid, DTOActualizarUsuario dtoActualizar)
        {
            HttpResponseMessage bloqueo = validarOwnership(guid);
            if (bloqueo != null) return bloqueo;

            ReqActualizarUsuario req = new ReqActualizarUsuario();
            req.guidUsuario = guid;
            req.nombre      = dtoActualizar.nombre;
            req.apellidos   = dtoActualizar.apellidos;
            return Request.CreateResponse(HttpStatusCode.OK, new LogUsuario().actualizarPerfil(req));
        }

        // DELETE api/usuarios/{guid}  — solo el propio usuario
        [HttpDelete]
        [Route("api/usuarios/{guid}")]
        public HttpResponseMessage eliminar(Guid guid)
        {
            HttpResponseMessage bloqueo = validarOwnership(guid);
            if (bloqueo != null) return bloqueo;

            ReqEliminarUsuario req = new ReqEliminarUsuario();
            req.guidUsuario = guid;
            return Request.CreateResponse(HttpStatusCode.OK, new LogUsuario().eliminar(req));
        }

        // PUT api/usuarios/{guid}/desactivar  — solo el propio usuario
        [HttpPut]
        [Route("api/usuarios/{guid}/desactivar")]
        public HttpResponseMessage desactivar(Guid guid)
        {
            HttpResponseMessage bloqueo = validarOwnership(guid);
            if (bloqueo != null) return bloqueo;

            ReqDesactivarUsuario req = new ReqDesactivarUsuario();
            req.guidUsuario = guid;
            return Request.CreateResponse(HttpStatusCode.OK, new LogUsuario().desactivar(req));
        }
    }
}

