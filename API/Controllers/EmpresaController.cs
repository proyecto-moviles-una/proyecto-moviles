using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Empresa;
using Logica.Empresa;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/empresa")]
    public class EmpresaController : ApiController
    {
        [HttpPost]
        [Route("crear")]
        public ResCrearEmpresa Crear([FromBody] DTOEmpresa dto)
        {
            var req = new ReqCrearEmpresa { Nombre = dto.Nombre, Telefono = dto.Telefono, Correo = dto.Correo };
            return new LogEmpresa().Crear(req);
        }

        [HttpGet]
        [Route("listar")]
        public ResListarEmpresas Listar()
        {
            return new LogEmpresa().Listar();
        }
    }
}
