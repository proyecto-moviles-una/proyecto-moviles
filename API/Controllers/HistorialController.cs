using System;
using System.Web.Http;
using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Historial;
using Logica.Historial;

namespace API.Controllers
{
    /// <summary>
    /// Controlador para gestionar historial de consultas
    /// </summary>
    [RoutePrefix("api/historial")]
    public class HistorialController : ApiController
    {

        /// <summary>
        /// Registrar consulta en historial
        /// </summary>
        [HttpPost]
        [Route("registrar")]
        public ResCrearHistorial Registrar(
            [FromBody] DTOHistorial dto)
        {
            ReqCrearHistorial req =
                new ReqCrearHistorial();

            req.GuidUsuario = dto.GuidUsuario;
            req.GuidRuta = dto.GuidRuta;

            return new LogHistorial().Crear(req);
        }



        /// <summary>
        /// Obtener historial de un usuario
        /// </summary>
        [HttpGet]
        [Route("usuario/{guid}")]
        public ResListarHistorial ListarPorUsuario(
            Guid guid)
        {
            return new LogHistorial()
                .ListarPorUsuario(guid);
        }

    }
}