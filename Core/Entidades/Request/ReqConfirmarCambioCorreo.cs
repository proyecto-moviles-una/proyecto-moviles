using System;

namespace Core.Entidades.Request
{
    public class ReqConfirmarCambioCorreo
    {
        public Guid   guidUsuario { get; set; }
        public string codigo      { get; set; }
    }
}
