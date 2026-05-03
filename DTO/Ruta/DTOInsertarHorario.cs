using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Ruta
{
    public class DTOInsertarHorario
    {
        public TimeSpan horaSalida { get; set; }
        public byte diasServicio { get; set; }
    }
}
