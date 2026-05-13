using System;

namespace Core.Entidades
{
    public class Ruta
    {
        public Guid Guid { get; set; }
        public Guid GuidEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public Guid? GuidZona { get; set; }
        public string NombreZona { get; set; }
        public string NumeroRuta { get; set; }
        public string Nombre { get; set; }
        public decimal? TarifaActual { get; set; }
        public Guid? GuidTarifa { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public byte EstadoServicio { get; set; }
    }
}
