
using System.Web.Http;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Logica.Parada;
using DTO.Parada;
using Core.Entidades;

namespace API.Controllers
{
    /// <summary>
    /// Controlador para gestionar operaciones CRUD de Paradas
    /// </summary>
    [RoutePrefix("api/parada")]
    public class ParadaController : ApiController
    {
        /// <summary>
        /// Crear una nueva parada
        /// </summary>
        /// <param name="dto">Datos de la parada a crear</param>
        /// <returns>Respuesta con la parada creada</returns>
        [HttpPost]
        [Route("crear")]
        public ResCrearParada Crear([FromBody] DTOParada dto)
        {
            ReqCrearParada req = new ReqCrearParada();

            req.Nombre = dto.Nombre;
            req.Descripcion = dto.Descripcion;
            req.Latitud = dto.Latitud;
            req.Longitud = dto.Longitud;

            return new LogParada().Crear(req);
        }

        /// <summary>
        /// Listar todas las paradas activas
        /// </summary>
        /// <returns>Lista de paradas</returns>
        [HttpGet]
        [Route("listar")]
        public ResListarParadas Listar()
        {
            return new LogParada().Listar();
        }
    }
}