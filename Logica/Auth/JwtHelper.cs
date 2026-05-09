using AccesoDatos;
using System;
using System.Linq;

namespace Logica.Auth
{
    public class TokenInfo
    {
        public bool   valido       { get; set; }
        public string guidUsuario  { get; set; }
        public string guidSesion   { get; set; }
        public string nombre       { get; set; }
        public string rol          { get; set; }
    }

    public static class JwtHelper
    {
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

        /// Valida que la sesion del JWT este activa en BD y pertenezca al usuario.
        /// Usa SP_VALIDAR_SESION (ya en DBML) que retorna GUID_USUARIO y ESTADO (bool).
        public static bool validarSesionEnBD(string guidSesion, string guidUsuario)
        {
            try
            {
                Guid sesion = Guid.Parse(guidSesion);
                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    SP_VALIDAR_SESIONResult sesionBD = linq.SP_VALIDAR_SESION(sesion).FirstOrDefault();
                    return sesionBD != null
                        && sesionBD.ESTADO == true
                        && sesionBD.GUID_USUARIO.ToString().Equals(guidUsuario, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}


