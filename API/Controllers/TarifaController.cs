using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Tarifa;
using Logica.Tarifa;
using System;
using System.Web.Http;

namespace API.Controllers
{
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
    }
}
