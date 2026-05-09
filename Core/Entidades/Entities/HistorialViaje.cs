using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Entities
{
    public class HistorialViaje
    {
        public Guid GuidHistorial { get; set; }
        public Guid GuidRuta { get; set; }
        public Guid GuidUsuario { get; set; }

        public decimal Tarifa { get; set; }

        public DateTime FechaConsulta { get; set; }

        public string NombreRuta { get; set; }
        public string DestinoRuta { get; set; }
    }
}

