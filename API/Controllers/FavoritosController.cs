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

        // GET api/favoritos  — lista los favoritos del usuario autenticado (guid viene del JWT)
        [HttpGet]
        [Route("api/favoritos")]
        public HttpResponseMessage obtener()
        {
            Guid guidUsuario = Guid.Parse(tokenActual.guidUsuario);
            ReqObtenerFavoritos req = new ReqObtenerFavoritos();
            req.guidUsuario = guidUsuario;
            return Request.CreateResponse(HttpStatusCode.OK, new LogFavorito().obtener(req));
        }

        // POST api/favoritos  — agrega un favorito al usuario autenticado (guid viene del JWT)
        [HttpPost]
        [Route("api/favoritos")]
        public HttpResponseMessage agregar(DTOAgregarFavorito dto)
        {
            Guid guidUsuario = Guid.Parse(tokenActual.guidUsuario);
            ReqAgregarFavorito req = new ReqAgregarFavorito();
            req.guidUsuario = guidUsuario;
            req.guidRuta    = dto.guidRuta;
            return Request.CreateResponse(HttpStatusCode.OK, new LogFavorito().agregar(req));
        }

        // DELETE api/favoritos/{guidFavorito}  — elimina un favorito del usuario autenticado
        [HttpDelete]
        [Route("api/favoritos/{guidFavorito}")]
        public HttpResponseMessage eliminar(Guid guidFavorito)
        {
            ReqEliminarFavorito req = new ReqEliminarFavorito();
            req.guidFavorito = guidFavorito;
            return Request.CreateResponse(HttpStatusCode.OK, new LogFavorito().eliminar(req));
        }
    }
}
