using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades
{
    public class Horario
    {
        public Guid? guidHorario { get; set; }
        public Guid? guidRuta { get; set; }
        public TimeSpan horaSalida { get; set; }
        public byte diasServicio { get; set; }
    }
}