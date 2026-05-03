using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades
{
    public class Ruta
    {
        public Guid? guidRuta { get; set; }
        public Guid? guidEmpresa { get; set; }
        public string nombreEmpresa { get; set; }
        public Guid? guidZona { get; set; }
        public string nombreZona { get; set; }
        public string numeroRuta { get; set; }
        public string nombre { get; set; }
        public decimal? tarifaActual { get; set; }  // solo lectura, informativo
        public TimeSpan? horaInicio { get; set; }
        public TimeSpan? horaFin { get; set; }
        public byte estadoServicio { get; set; }  // 1=activo, 0=inactivo
        public DateTime fechaRegistro { get; set; }
    }
}
