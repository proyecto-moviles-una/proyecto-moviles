using System;

namespace Core.Entidades.Request
{
    public class ReqAsociarParadaRuta
    {
        public Guid GuidRuta { get; set; }
        public Guid GuidParada { get; set; }
        public int Orden { get; set; }
    }
}
