using System;
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

        // -------- LISTAR CERCANAS --------
        [HttpGet]
        [Route("cercanas")]
        public ResListarParadas ListarCercanas(decimal latitud, decimal longitud, decimal radioKm = 1)
        {
            return new LogParada().ListarCercanas(latitud, longitud, radioKm);
        }

        // -------- OBTENER --------
        [HttpGet]
        [Route("obtener/{guid}")]
        public ResCrearParada Obtener(string guid)
        {
            return new LogParada().ObtenerPorGuid(guid);
        }

        // -------- LISTAR CERCANAS POR ZONA --------
        [HttpGet]
        [Route("cercanas-por-zona")]
        public ResListarParadasConRutas ListarCercanasConRutasPorZona(string guidZona = null, decimal? latitud = null, decimal? longitud = null, decimal radioKm = 5)
        {
            Guid guid = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(guidZona))
                Guid.TryParse(guidZona, out guid);

            return new LogParada().ListarCercanasConRutasPorZona(latitud, longitud, radioKm, guid);
        }

        // -------- LISTAR POR RUTA --------
        [HttpGet]
        [Route("ruta/{guidRuta}")]
        public ResListarParadas ListarPorRuta(string guidRuta)
        {
            return new LogParada().ListarPorRuta(guidRuta);
        }

        // -------- LISTAR RUTAS POR PARADA --------
        [HttpGet]
        [Route("{guidParada}/rutas")]
        public ResListarRutas ListarRutasPorParada(string guidParada)
        {
            return new LogParada().ListarRutasPorParada(guidParada);
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
