using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Request
{
    public class ReqActualizarRuta
    {
        public Guid guidRuta { get; set; }  // obligatorio
        public string nombre { get; set; }  // obligatorio
        public string numeroRuta { get; set; }
        public TimeSpan? horaInicio { get; set; }
        public TimeSpan? horaFin { get; set; }
        public byte estadoServicio { get; set; }
    }
}
