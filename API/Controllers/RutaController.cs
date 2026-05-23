using API.Filters;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Ruta;
using Logica.Ruta;
using System;
using System.Web.Http;

namespace API.Controllers
{
    [JwtAuth]
    [RoutePrefix("api/ruta")]
    public class RutaController : ApiController
    {
        [HttpPost]
        [Route("crear")]
        public ResCrearRuta Crear([FromBody] DTORuta dto)
        {
            var req = new ReqCrearRuta
            {
                GuidEmpresa = new Guid(dto.GuidEmpresa),
                GuidZona = string.IsNullOrEmpty(dto.GuidZona) ? (Guid?)null : new Guid(dto.GuidZona),
                NumeroRuta = dto.NumeroRuta,
                Nombre = dto.Nombre,
                HoraInicio = string.IsNullOrEmpty(dto.HoraInicio) ? (TimeSpan?)null : TimeSpan.Parse(dto.HoraInicio),
                HoraFin = string.IsNullOrEmpty(dto.HoraFin) ? (TimeSpan?)null : TimeSpan.Parse(dto.HoraFin)
            };
            return new LogRuta().Crear(req);
        }

        [HttpGet]
        [Route("listar")]
        public ResListarRutas Listar()
        {
            return new LogRuta().Listar();
        }

        [HttpGet]
        [Route("obtener/{guid}")]
        public ResCrearRuta ObtenerPorGuid(string guid)
        {
            return new LogRuta().ObtenerPorGuid(new Guid(guid));
        }

        [HttpPut]
        [Route("editar/{guid}")]
        public ResCrearRuta Editar(string guid, [FromBody] DTORuta dto)
        {
            var req = new ReqCrearRuta
            {
                GuidEmpresa = new Guid(dto.GuidEmpresa),
                GuidZona = string.IsNullOrEmpty(dto.GuidZona) ? (Guid?)null : new Guid(dto.GuidZona),
                NumeroRuta = dto.NumeroRuta,
                Nombre = dto.Nombre,
                HoraInicio = string.IsNullOrEmpty(dto.HoraInicio) ? (TimeSpan?)null : TimeSpan.Parse(dto.HoraInicio),
                HoraFin = string.IsNullOrEmpty(dto.HoraFin) ? (TimeSpan?)null : TimeSpan.Parse(dto.HoraFin)
            };
            return new LogRuta().Editar(new Guid(guid), req);
        }

        [HttpDelete]
        [Route("eliminar/{guid}")]
        public ResBase Eliminar(string guid)
        {
            return new LogRuta().Eliminar(new Guid(guid));
        }

        [HttpPost]
        [Route("asociar-parada/{guidRuta}")]
        public ResAsociarParadaRuta AsociarParada(string guidRuta, [FromBody] DTOAsociarParadaRuta dto)
        {
            var req = new ReqAsociarParadaRuta
            {
                GuidRuta = new Guid(guidRuta),
                GuidParada = new Guid(dto.GuidParada),
                Orden = dto.Orden
            };
            return new LogRuta().AsociarParada(req);
        }
        [HttpDelete]
        [Route("desasociar-parada/{guidRuta}/{guidParada}")]
        public ResBase DesasociarParada(string guidRuta, string guidParada)
        {
            var req = new ReqDesasociarParadaRuta
            {
                GuidRuta = new Guid(guidRuta),
                GuidParada = new Guid(guidParada)
            };
            return new LogRuta().DesasociarParada(req);
        }
    }
}

