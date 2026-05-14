using API.Filters;
using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Historial;
using Logica.Auth;
using Logica.Historial;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    /// <summary>
    /// Controlador para gestionar historial de consultas
    /// </summary>
    [JwtAuth]
    [RoutePrefix("api/historial")]
    public class HistorialController : ApiController
    {
        private TokenInfo tokenActual =>
            Request.Properties.ContainsKey("tokenInfo")
                ? (TokenInfo)Request.Properties["tokenInfo"]
                : null;

        private bool TryObtenerUsuarioToken(out Guid guidUsuario)
        {
            guidUsuario = Guid.Empty;

            return tokenActual != null &&
                   Guid.TryParse(tokenActual.guidUsuario, out guidUsuario);
        }

        private HttpResponseMessage validarOwnership(Guid guidUsuario)
        {
            if (tokenActual == null ||
                !string.Equals(tokenActual.guidUsuario, guidUsuario.ToString(), StringComparison.OrdinalIgnoreCase))
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                    "No tiene permiso para acceder a este recurso.");

            return null;
        }

        /// <summary>
        /// Registrar consulta en historial para el usuario autenticado
        /// </summary>
        [HttpPost]
        [Route("registrar")]
        public HttpResponseMessage Registrar([FromBody] DTOHistorial dto)
        {
            Guid guidUsuario;
            if (!TryObtenerUsuarioToken(out guidUsuario))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Token inválido o sin usuario asociado.");
            }

            ReqCrearHistorial req = new ReqCrearHistorial();

            req.GuidUsuario = guidUsuario;
            req.GuidRuta = dto != null ? dto.GuidRuta : Guid.Empty;

            return Request.CreateResponse(HttpStatusCode.OK, new LogHistorial().Crear(req));
        }

        /// <summary>
        /// Obtener historial del usuario autenticado
        /// </summary>
        [HttpGet]
        [Route("mio")]
        public HttpResponseMessage ListarMio()
        {
            Guid guidUsuario;
            if (!TryObtenerUsuarioToken(out guidUsuario))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Token inválido o sin usuario asociado.");
            }

            return Request.CreateResponse(HttpStatusCode.OK, new LogHistorial().ListarPorUsuario(guidUsuario));
        }

        /// <summary>
        /// Obtener historial de un usuario validando que sea el dueño del token
        /// </summary>
        [HttpGet]
        [Route("usuario/{guid}")]
        public HttpResponseMessage ListarPorUsuario(Guid guid)
        {
            HttpResponseMessage bloqueo = validarOwnership(guid);
            if (bloqueo != null) return bloqueo;

            return Request.CreateResponse(HttpStatusCode.OK, new LogHistorial().ListarPorUsuario(guid));
        }
    }
}
