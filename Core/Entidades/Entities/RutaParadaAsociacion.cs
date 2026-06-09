using System;

namespace Core.Entidades
{
    public class RutaParadaAsociacion
    {
        public Guid GuidRuta { get; set; }
        public Guid GuidParada { get; set; }
        public int? Orden { get; set; }
    }
}
