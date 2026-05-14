using System.Web.Http;
using API.Filters;
using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Parada;
using Logica.Parada;

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
        [JwtAuth]
        [HttpPost]
        [Route("crear")]
        public ResCrearParada Crear([FromBody] DTOParada dto)
        {
            dto = dto ?? new DTOParada();

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

        // -------- OBTENER --------
        [HttpPost]
        [Route("obtener")]
        public ResObtenerParada Obtener([FromBody] DTOObtenerParada dto)
        {
            dto = dto ?? new DTOObtenerParada();

            ReqObtenerParada req = new ReqObtenerParada();
            req.Guid = dto.Guid;

            return new LogParada().Obtener(req);
        }

        // -------- LISTAR POR RUTA --------
        [HttpPost]
        [Route("ruta")]
        public ResListarParadas ListarPorRuta([FromBody] DTOListarParadasPorRuta dto)
        {
            dto = dto ?? new DTOListarParadasPorRuta();

            ReqListarParadasPorRuta req = new ReqListarParadasPorRuta();
            req.GuidRuta = dto.GuidRuta;

            return new LogParada().ListarPorRuta(req);
        }

        // -------- EDITAR --------
        [JwtAuth]
        [HttpPut]
        [Route("editar/{guid}")]
        public ResEditarParada Editar(string guid, [FromBody] DTOParada dto)
        {
            dto = dto ?? new DTOParada();

            ReqEditarParada req = new ReqEditarParada();
            req.Guid = guid;
            req.Nombre = dto.Nombre;
            req.Descripcion = dto.Descripcion;
            req.Latitud = dto.Latitud;
            req.Longitud = dto.Longitud;

            return new LogParada().Editar(req);
        }

        // -------- ELIMINAR --------
        [JwtAuth]
        [HttpDelete]
        [Route("eliminar/{guid}")]
        public ResEliminarParada Eliminar(string guid)
        {
            ReqEliminarParada req = new ReqEliminarParada();
            req.Guid = guid;

            return new LogParada().Eliminar(req);
        }
    }
}
