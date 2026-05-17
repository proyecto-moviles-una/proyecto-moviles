using API.Filters;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Tarifa;
using Logica.Tarifa;
using System;
using System.Web.Http;

namespace API.Controllers
{
    [JwtAuth]
    [RoutePrefix("api/tarifa")]
    public class TarifaController : ApiController
    {
        [HttpPost]
        [Route("crear")]
        public ResCrearTarifa Crear([FromBody] DTOTarifa dto)
        {
            var req = new ReqCrearTarifa
            {
                GuidRuta = new Guid(dto.GuidRuta),
                Monto = dto.Monto,
                FechaVigencia = dto.FechaVigencia
            };
            return new LogTarifa().Crear(req);
        }

        [HttpGet]
        [Route("listar/{guidRuta}")]
        public ResListarTarifas Listar(string guidRuta)
        {
            return new LogTarifa().Listar(new Guid(guidRuta));
        }

        [HttpGet]
        [Route("obtener/{guid}")]
        public ResCrearTarifa ObtenerPorGuid(string guid)
        {
            return new LogTarifa().ObtenerPorGuid(new Guid(guid));
        }

        [HttpPut]
        [Route("editar/{guid}")]
        public ResCrearTarifa Editar(string guid, [FromBody] DTOTarifa dto)
        {
            var req = new ReqCrearTarifa
            {
                Monto = dto.Monto,
                FechaVigencia = dto.FechaVigencia
            };
            return new LogTarifa().Editar(new Guid(guid), req);
        }

        [HttpDelete]
        [Route("eliminar/{guid}")]
        public ResBase Eliminar(string guid)
        {
            return new LogTarifa().Eliminar(new Guid(guid));
        }
    }
}
