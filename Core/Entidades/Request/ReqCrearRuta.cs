using System;

namespace Core.Entidades.Request
{
    public class ReqCrearRuta
    {
        public Guid GuidEmpresa { get; set; }
        public Guid? GuidZona { get; set; }
        public string NumeroRuta { get; set; }
        public string Nombre { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
    }
}
