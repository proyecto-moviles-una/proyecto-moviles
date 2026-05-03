using API.Filters;
using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Zona;
using Logica.Zona;
using System.Web.Http;

namespace API.Controllers
{
    public class ZonasController : ApiController
    {
        // GET api/zonas — Público: listar todas las zonas
        [HttpGet]
        [Route("api/zonas")]
        public ResObtenerZonas listar()
        {
            return new LogZona().obtener(new ReqObtenerZonas());
        }

        // POST api/zonas — JWT Admin: crear zona
        [JwtAuth]
        [HttpPost]
        [Route("api/zonas")]
        public ResInsertarZona insertar([FromBody] DTOInsertarZona dto)
        {
            ReqInsertarZona req = new ReqInsertarZona();
            req.nombre      = dto.nombre;
            req.descripcion = dto.descripcion;
            return new LogZona().insertar(req);
        }
    }
}
