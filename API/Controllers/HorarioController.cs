using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Horario;
using Logica.Horario;
using System;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/horario")]
    public class HorarioController : ApiController
    {
        [HttpPost]
        [Route("crear")]
        public ResCrearHorario Crear([FromBody] DTOHorario dto)
        {
            var req = new ReqCrearHorario
            {
                GuidRuta = new Guid(dto.GuidRuta),
                HoraSalida = TimeSpan.Parse(dto.HoraSalida),
                DiasServicio = dto.DiasServicio
            };
            return new LogHorario().Crear(req);
        }

        [HttpGet]
        [Route("listar/{guidRuta}")]
        public ResListarHorarios ListarPorRuta(string guidRuta)
        {
            return new LogHorario().ListarPorRuta(new Guid(guidRuta));
        }
    }
}
