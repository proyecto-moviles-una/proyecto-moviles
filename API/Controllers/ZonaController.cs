using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Zona;
using Logica.Zona;
using System.Web.Http;

namespace API.Controllers
{
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
    }
}
