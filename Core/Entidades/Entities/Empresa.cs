using System;

namespace Core.Entidades
{
    public class Empresa
    {
        public Guid Guid { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public bool Estado { get; set; }
    }
}
