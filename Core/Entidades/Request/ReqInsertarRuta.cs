using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Request
{
    public class ReqInsertarRuta
    {
        public Guid guidEmpresa { get; set; }  // obligatorio
        public Guid? guidZona { get; set; }
        public string numeroRuta { get; set; }
        public string nombre { get; set; }  // obligatorio
        public TimeSpan? horaInicio { get; set; }
        public TimeSpan? horaFin { get; set; }
        public byte estadoServicio { get; set; }  // default 1
    }
}
