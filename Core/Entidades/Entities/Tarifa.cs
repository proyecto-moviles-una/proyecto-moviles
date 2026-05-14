using System;

namespace Core.Entidades
{
    public class Tarifa
    {
        public Guid Guid { get; set; }
        public Guid GuidRuta { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaVigencia { get; set; }
        public bool Estado { get; set; }
    }
}
