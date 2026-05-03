using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Request
{
    public class ReqInsertarHorario
    {
        public Guid guidRuta { get; set; }  // obligatorio
        public TimeSpan horaSalida { get; set; }  // obligatorio
        public byte diasServicio { get; set; }  // obligatorio, bitmap
    }
}
