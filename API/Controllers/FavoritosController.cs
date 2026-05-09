using API.Filters;
using Core.Entidades.Request;
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
    public class FavoritosController : ApiController
    {
        // Extrae el TokenInfo inyectado por JwtAuthFilter
        private TokenInfo tokenActual =>
            Request.Properties.ContainsKey("tokenInfo")
                ? (TokenInfo)Request.Properties["tokenInfo"]
                : null;

        // Valida que el guid de la URL sea el mismo del token
        private HttpResponseMessage validarOwnership(Guid guid)
        {
            if (tokenActual == null ||
                !string.Equals(tokenActual.guidUsuario, guid.ToString(), StringComparison.OrdinalIgnoreCase))
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                    "No tiene permiso para acceder a este recurso.");
            return null;
        }

        // GET api/favoritos/{guidUsuario}  — lista los favoritos del usuario
        [HttpGet]
        [Route("api/favoritos/{guidUsuario}")]
        public HttpResponseMessage obtener(Guid guidUsuario)
        {
            HttpResponseMessage bloqueo = validarOwnership(guidUsuario);
            if (bloqueo != null) return bloqueo;

            ReqObtenerFavoritos req = new ReqObtenerFavoritos();
            req.guidUsuario = guidUsuario;
            return Request.CreateResponse(HttpStatusCode.OK, new LogFavorito().obtener(req));
        }

        // POST api/favoritos/{guidUsuario}  — agrega un favorito
        [HttpPost]
        [Route("api/favoritos/{guidUsuario}")]
        public HttpResponseMessage agregar(Guid guidUsuario, DTOAgregarFavorito dto)
        {
            HttpResponseMessage bloqueo = validarOwnership(guidUsuario);
            if (bloqueo != null) return bloqueo;

            ReqAgregarFavorito req = new ReqAgregarFavorito();
            req.guidUsuario = guidUsuario;
            req.guidRuta    = dto.guidRuta;
            return Request.CreateResponse(HttpStatusCode.OK, new LogFavorito().agregar(req));
        }

        // DELETE api/favoritos/{guidUsuario}/{guidFavorito}  — elimina un favorito
        [HttpDelete]
        [Route("api/favoritos/{guidUsuario}/{guidFavorito}")]
        public HttpResponseMessage eliminar(Guid guidUsuario, Guid guidFavorito)
        {
            HttpResponseMessage bloqueo = validarOwnership(guidUsuario);
            if (bloqueo != null) return bloqueo;

            ReqEliminarFavorito req = new ReqEliminarFavorito();
            req.guidFavorito = guidFavorito;
            return Request.CreateResponse(HttpStatusCode.OK, new LogFavorito().eliminar(req));
        }
    }
}
