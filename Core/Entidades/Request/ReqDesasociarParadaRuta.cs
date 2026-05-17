using System;

namespace Core.Entidades.Request
{
    public class ReqDesasociarParadaRuta
    {
        public Guid GuidRuta { get; set; }
        public Guid GuidParada { get; set; }
    }
}
