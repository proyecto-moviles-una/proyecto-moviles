using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades
{
    public class Parada
    { 
        public Guid guid { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public int? Orden { get; set; }
        public decimal? DistanciaKm { get; set; }
    }
}
