using System;

namespace Core.Entidades.Request
{
    /// <summary>
    /// Request del metodo LogUsuario.actualizarTokenFCM.
    /// La app MAUI lo envia al arrancar para que el backend tenga el token FCM vigente.
    /// </summary>
    public class ReqActualizarTokenFCM
    {
        public Guid guidUsuario { get; set; }
        public string tokenFCM { get; set; }
    }
}
