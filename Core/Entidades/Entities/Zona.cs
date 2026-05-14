using System;

namespace Core.Entidades
{
    public class Zona
    {
        public Guid Guid { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
