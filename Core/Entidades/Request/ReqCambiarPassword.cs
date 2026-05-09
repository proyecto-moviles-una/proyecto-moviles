using System;

namespace Core.Entidades.Request
{
    public class ReqCambiarPassword
    {
        public Guid   guidUsuario       { get; set; }
        public string passwordActual    { get; set; }
        public string passwordNueva     { get; set; }
        public string confirmarPassword { get; set; }
    }
}
