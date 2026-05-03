using API.Filters;
using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Empresa;
using Logica.Empresa;
using System.Web.Http;

namespace API.Controllers
{
    public class EmpresasController : ApiController
    {
        // GET api/empresas — Público: listar todas las empresas
        [HttpGet]
        [Route("api/empresas")]
        public ResObtenerEmpresas listar()
        {
            return new LogEmpresa().obtener(new ReqObtenerEmpresas());
        }

        // POST api/empresas — JWT Admin: crear empresa
        [JwtAuth]
        [HttpPost]
        [Route("api/empresas")]
        public ResInsertarEmpresa insertar([FromBody] DTOInsertarEmpresa dto)
        {
            ReqInsertarEmpresa req = new ReqInsertarEmpresa();
            req.nombre   = dto.nombre;
            req.telefono = dto.telefono;
            req.correo   = dto.correo;
            return new LogEmpresa().insertar(req);
        }
    }
}
