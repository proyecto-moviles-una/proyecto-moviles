using API.Filters;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Horario;
using Logica.Horario;
using System;
using System.Web.Http;

namespace API.Controllers
{
    [JwtAuth]
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

        [HttpGet]
        [Route("obtener/{guid}")]
        public ResCrearHorario ObtenerPorGuid(string guid)
        {
            return new LogHorario().ObtenerPorGuid(new Guid(guid));
        }

        [HttpPut]
        [Route("editar/{guid}")]
        public ResCrearHorario Editar(string guid, [FromBody] DTOHorario dto)
        {
            var req = new ReqCrearHorario
            {
                HoraSalida = TimeSpan.Parse(dto.HoraSalida),
                DiasServicio = dto.DiasServicio
            };
            return new LogHorario().Editar(new Guid(guid), req);
        }

        [HttpDelete]
        [Route("eliminar/{guid}")]
        public ResBase Eliminar(string guid)
        {
            return new LogHorario().Eliminar(new Guid(guid));
        }
    }
}
