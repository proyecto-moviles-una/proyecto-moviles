using System;

namespace Core.Entidades.Request
{
    public class ReqCrearTarifa
    {
        public Guid GuidRuta { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaVigencia { get; set; }
    }
}
