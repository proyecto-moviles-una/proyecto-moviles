using API.Filters;
using Core.Entidades.Request;
using Core.Entidades;
using Core.Entidades.Response;
using DTO.Empresa;
using Logica.Empresa;
using System;
using System.Web.Http;

namespace API.Controllers
{
    [JwtAuth]
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

        [HttpGet]
        [Route("obtener/{guid}")]
        public ResCrearEmpresa ObtenerPorGuid(string guid)
        {
            return new LogEmpresa().ObtenerPorGuid(new Guid(guid));
        }

        [HttpPut]
        [Route("editar/{guid}")]
        public ResCrearEmpresa Editar(string guid, [FromBody] DTOEmpresa dto)
        {
            var req = new ReqCrearEmpresa { Nombre = dto.Nombre, Telefono = dto.Telefono, Correo = dto.Correo };
            return new LogEmpresa().Editar(new Guid(guid), req);
        }

        [HttpDelete]
        [Route("eliminar/{guid}")]
        public ResBase Eliminar(string guid)
        {
            return new LogEmpresa().Eliminar(new Guid(guid));
        }
    }
}
