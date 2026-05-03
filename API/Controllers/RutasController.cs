using API.Filters;
using Core.Entidades.Request;
using Core.Entidades.Response;
using DTO.Ruta;
using Logica.Ruta;
using System;
using System.Web.Http;

namespace API.Controllers
{
    public class RutasController : ApiController
    {
        // GET api/rutas — Público: lista todas las rutas activas
        [HttpGet]
        [Route("api/rutas")]
        public ResObtenerRutas listar()
        {
            ReqObtenerRutas req = new ReqObtenerRutas();
            req.soloActivas = true;
            return new LogRuta().obtener(req);
        }

        // GET api/rutas/{guid} — Público: detalle de una ruta
        [HttpGet]
        [Route("api/rutas/{guid}")]
        public ResObtenerRuta obtener(Guid guid)
        {
            return new LogRuta().obtenerPorGuid(guid);
        }
        

        // GET api/rutas/{guid}/horarios — Público: horarios de una ruta (RF-05)
        [HttpGet]
        [Route("api/rutas/{guid}/horarios")]
        public IHttpActionResult obtenerHorarios(Guid guid)
        {
            // Pendiente RF-05: requiere SP_OBTENER_HORARIOS_POR_RUTA en el DBML
            return Ok(new { resultado = false, mensaje = "Pendiente implementación RF-05" });
        }

        // POST api/rutas — JWT Admin: crear ruta
        [JwtAuth]
        [HttpPost]
        [Route("api/rutas")]
        public ResInsertarRuta insertar([FromBody] DTOInsertarRuta dto)
        {
            ReqInsertarRuta req = new ReqInsertarRuta();
            req.guidEmpresa = dto.guidEmpresa;
            req.guidZona = dto.guidZona;
            req.numeroRuta = dto.numeroRuta;
            req.nombre = dto.nombre;
            req.horaInicio = dto.horaInicio;
            req.horaFin = dto.horaFin;
            req.estadoServicio = dto.estadoServicio;
            return new LogRuta().insertar(req);
        }

        // PUT api/rutas/{guid} — JWT Admin: editar ruta
        [JwtAuth]
        [HttpPut]
        [Route("api/rutas/{guid}")]
        public ResActualizarRuta actualizar(Guid guid, [FromBody] DTOActualizarRuta dto)
        {
            ReqActualizarRuta req = new ReqActualizarRuta();
            req.guidRuta = guid;
            req.nombre = dto.nombre;
            req.numeroRuta = dto.numeroRuta;
            req.horaInicio = dto.horaInicio;
            req.horaFin = dto.horaFin;
            req.estadoServicio = dto.estadoServicio;
            return new LogRuta().actualizar(req);
        }

        // DELETE api/rutas/{guid} — JWT Admin: desactivar ruta (soft delete)
        [JwtAuth]
        [HttpDelete]
        [Route("api/rutas/{guid}")]
        public ResDesactivarRuta desactivar(Guid guid)
        {
            ReqDesactivarRuta req = new ReqDesactivarRuta();
            req.guidRuta = guid;
            return new LogRuta().desactivar(req);
        }
    }
}