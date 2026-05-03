using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Ruta
{
    public class DTOInsertarRuta
    {
        public Guid guidEmpresa { get; set; }
        public Guid? guidZona { get; set; }
        public string numeroRuta { get; set; }
        public string nombre { get; set; }
        public TimeSpan? horaInicio { get; set; }
        public TimeSpan? horaFin { get; set; }
        public byte estadoServicio { get; set; }
    }
}
