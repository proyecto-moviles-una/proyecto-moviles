using System;

namespace Core.Entidades.Request
{
    public class ReqSolicitarCambioCorreo
    {
        public Guid   guidUsuario    { get; set; }
        public string passwordActual { get; set; }
        public string correoNuevo    { get; set; }
    }
}
