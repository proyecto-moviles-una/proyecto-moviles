using API.Filters;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Zona;
using Logica.Zona;
using System.Web.Http;

namespace API.Controllers
{
    [JwtAuth]
    [RoutePrefix("api/zona")]
    public class ZonaController : ApiController
    {
        [HttpPost]
        [Route("crear")]
        public ResCrearZona Crear([FromBody] DTOZona dto)
        {
            var req = new ReqCrearZona { Nombre = dto.Nombre, Descripcion = dto.Descripcion };
            return new LogZona().Crear(req);
        }

        [HttpGet]
        [Route("listar")]
        public ResListarZonas Listar()
        {
            return new LogZona().Listar();
        }

        [HttpGet]
        [Route("obtener/{guid}")]
        public ResCrearZona ObtenerPorGuid(string guid)
        {
            return new LogZona().ObtenerPorGuid(new System.Guid(guid));
        }

        [HttpPut]
        [Route("editar/{guid}")]
        public ResCrearZona Editar(string guid, [FromBody] DTOZona dto)
        {
            var req = new ReqCrearZona { Nombre = dto.Nombre, Descripcion = dto.Descripcion };
            return new LogZona().Editar(new System.Guid(guid), req);
        }

        [HttpDelete]
        [Route("eliminar/{guid}")]
        public ResBase Eliminar(string guid)
        {
            return new LogZona().Eliminar(new System.Guid(guid));
        }
    }
}
