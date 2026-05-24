using System;

namespace Core.Entidades.Request
{
    public class ReqActualizarTokenFCM
    {
        public Guid   guidUsuario { get; set; }
        public string tokenFCM    { get; set; }
    }
}
