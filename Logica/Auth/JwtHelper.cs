namespace Logica.Auth
{
    /// <summary>
    /// Resultado simplificado de la validacion del JWT.
    /// Se expone aqui para que API no necesite referenciar directamente Utilitarios.
    /// </summary>
    public class TokenInfo
    {
        public bool   valido       { get; set; }
        public string guidUsuario  { get; set; }
        public string guidSesion   { get; set; }
        public string nombre       { get; set; }
        public string rol          { get; set; }
    }

    /// <summary>
    /// Wrapper de validacion JWT accesible desde el proyecto API.
    /// </summary>
    public static class JwtHelper
    {
        /// <summary>
        /// Valida el token JWT y retorna la info del usuario, o valido=false si es invalido/expirado.
        /// </summary>
        public static TokenInfo validarToken(string token)
        {
            Utilitarios.JwtPayload payload = Utilitarios.Utilitarios.validarJWT(token);

            if (payload == null)
                return new TokenInfo { valido = false };

            return new TokenInfo
            {
                valido      = true,
                guidUsuario = payload.guidUsuario,
                guidSesion  = payload.guidSesion,
                nombre      = payload.nombre,
                rol         = payload.rol
            };
        }
    }
}
