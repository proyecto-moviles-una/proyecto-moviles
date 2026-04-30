using AccesoDatos;
using BCrypt.Net;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Enum;
using System;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace Utilitarios
{
    public static class Utilitarios
    {
        // ?????????????????????????????????????????????????????????????????????
        // CONFIGURACIÓN
        // ????????????????????????????????????????????????????????????????????? OJO SI EL PROFE PREGUNTA DE STO, LE COMENTO QUE LO MEJRO E SPASARLO A UN .ENV POR LA SEGURIRDAD
        private const string JWT_SECRET  = "MiBus@SecretKey2025!AlajuelaCR";
        private const int    JWT_MINUTOS = 120;   // 2 horas de vigencia

        private const string SMTP_HOST     = "smtp.gmail.com";
        private const int    SMTP_PORT     = 587;
        private const string SMTP_USUARIO  = "nayidelin.jiron.castellon@est.una.ac.cr";   // ? PON TU CORREO GMAIL REAL
        private const string SMTP_PASSWORD = "iejk xvxq puzv ipjy";         // ? PON TU APP PASSWORD DE GMAIL (no la contraseña normal)

        // ?????????????????????????????????????????????????????????????????????
        // ERRORES
        // ?????????????????????????????????????????????????????????????????????

        /// <summary>Crea un objeto Error con código y mensaje legible.</summary>
        public static Error crearError(enumErrores codigo) // ste metodo recive un codigo de error
        {
            return new Error
            {
                codigo  = codigo,
                mensaje = obtenerMensajeError(codigo) // y devuelve un objeto con error
            };
        }

        private static string obtenerMensajeError(enumErrores codigo)
        {
            switch (codigo)
            {
                case enumErrores.nombreFaltante:       return "El nombre es obligatorio.";
                case enumErrores.apellidosFaltante:    return "Los apellidos son obligatorios.";
                case enumErrores.emailFaltante:        return "El correo electrónico es obligatorio.";
                case enumErrores.emailInvalido:        return "El formato del correo electrónico no es válido.";
                case enumErrores.passwordVacio:        return "La contraseña es obligatoria.";
                case enumErrores.guidDeUsuarioFaltante:return "El identificador de usuario es obligatorio.";
                case enumErrores.errorActivandoUsuario:return "No se pudo activar el usuario. Verifique el enlace.";
                case enumErrores.loginIncorrecto:      return "Correo o contraseña incorrectos.";
                case enumErrores.usuarioInactivo:       return "La cuenta no ha sido verificada. Revise su correo.";
                case enumErrores.usuarioDesactivado:    return "Esta cuenta ha sido desactivada.";
                case enumErrores.codigoExpirado:        return "El código expiró. Solicite un nuevo registro.";
                case enumErrores.correoYaRegistrado:    return "El correo ya está registrado.";
                case enumErrores.guidSesionFaltante:    return "El identificador de sesión es obligatorio.";
                case enumErrores.sesionInvalida:        return "La sesión no es válida o ha expirado.";
                case enumErrores.errorAbrirSesion:      return "No se pudo abrir la sesión.";
                case enumErrores.sesionYaCerrada:       return "La sesión ya fue cerrada anteriormente.";
                case enumErrores.accesoNoAutorizado:    return "No tiene permiso para acceder a este recurso.";
                case enumErrores.guidRutaFaltante:     return "El identificador de ruta es obligatorio.";
                case enumErrores.favoritoYaExiste:     return "Esta ruta ya está en sus favoritos.";
                case enumErrores.favoritoNoExiste:     return "El favorito indicado no existe.";
                case enumErrores.guidFavoritoFaltante: return "El identificador de favorito es obligatorio.";
                case enumErrores.errorBaseDatos:       return "Error en base de datos. Intente más tarde.";
                default:                               return "Ha ocurrido un error inesperado.";
            }
        }

        // ?????????????????????????????????????????????????????????????????????
        // BITÁCORA
        // ?????????????????????????????????????????????????????????????????????

        /// <summary>Inserta un registro en TB_BITACORA mediante SP_INSERTAR_BITACORA.</summary>
        public static void bitacorear(ReqBitacorear req)
        {
            try
            {
                using (ConexionLinqDataContext linq = new ConexionLinqDataContext())
                {
                    System.Nullable<System.Guid> guidUsr = req.bitacora.guidUsuario;
                    linq.SP_INSERTAR_BITACORA(
                        guidUsr,
                        req.bitacora.clase,
                        req.bitacora.metodo,
                        (short)req.bitacora.tipo,
                        req.bitacora.errorId,
                        req.bitacora.descripcion,
                        req.bitacora.request,
                        req.bitacora.response
                    );
                }
            }
            catch
            {
                // La bitácora no debe lanzar excepción hacia arriba
            }
        }

        // ?????????????????????????????????????????????????????????????????????
        // TOKENS Y SEGURIDAD
        // ?????????????????????????????????????????????????????????????????????

        /// <summary>Genera un código de verificación de 6 caracteres alfanuméricos (igual que el profesor).</summary>
        public static string crearToken()
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] codigo = new char[6];
            for (int i = 0; i < 6; i++)
                codigo[i] = caracteres[random.Next(caracteres.Length)];
            return new string(codigo);
        }

        /// <summary>
        /// Genera hash BCrypt de la contraseña.
        /// BCrypt incluye su propio salt aleatorio — no necesitamos uno externo.
        /// WorkFactor 12 = ~300ms por hash (balance seguridad/rendimiento).
        /// </summary>
        public static string hashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        /// <summary>Verifica que la contraseña coincida con el hash BCrypt almacenado.</summary>
        public static bool verificarPassword(string password, string hashGuardado)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashGuardado);
            }
            catch
            {
                return false;
            }
        }

        // ?????????????????????????????????????????????????????????????????????
        // JWT
        // ?????????????????????????????????????????????????????????????????????

        /// <summary>
        /// Genera un JWT firmado con HMACSHA256.
        /// Payload: guidUsuario, guidSesion, nombre, rol, exp.
        /// </summary>
        /// Este método crea el token que el usuario recibe al hacer login.
        public static string generarJWT(Guid guidUsuario, Guid guidSesion, string nombre, string rol = "usuario")
        {
            string headerJson  = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";
            long   expUnix     = DateTimeOffset.UtcNow.AddMinutes(JWT_MINUTOS).ToUnixTimeSeconds();
            string payloadJson = string.Format( 
                "{{\"guidUsuario\":\"{0}\",\"guidSesion\":\"{1}\",\"nombre\":\"{2}\",\"rol\":\"{3}\",\"exp\":{4}}}", // ese Payload es el contenido del token
                guidUsuario, guidSesion, nombre.Replace("\"", "\\\""), rol, expUnix);

            string headerB64  = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            string payloadB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
            string firma      = firmarHMACSHA256($"{headerB64}.{payloadB64}", JWT_SECRET); // aqui pues usa el secrt de JWT para genera una firma, unica y nadie lo puedo modificar

            return $"{headerB64}.{payloadB64}.{firma}";
        }

        /// <summary>
        /// Valida el JWT y retorna el payload, o null si es inválido/expirado.
        /// </summary>
        public static JwtPayload validarJWT(string token) // DEVE CONTENER 3 PARTE  PARA QUE SEA VALIDO
            // VAIDAR LA FIRMA.  LEER EL PAYLOAD, PARSEAR, Y VALDAR LA EXPERACION SI TODO ESTA BIEN DEVUELVE EL PAYLOT 
            // osea la info del usario
        {
            try
            {
                string[] partes = token.Split('.');
                if (partes.Length != 3) return null;

                string firmaEsperada = firmarHMACSHA256($"{partes[0]}.{partes[1]}", JWT_SECRET);
                if (firmaEsperada != partes[2]) return null;

                string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(partes[1]));
                JwtPayload payload = parsearPayload(payloadJson);

                long ahora = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (payload == null || payload.exp < ahora) return null;

                return payload;
            }
            catch
            {
                return null;
            }
        }

        private static JwtPayload parsearPayload(string json)
        {
            // Parser manual simple — evita dependencia de Newtonsoft en esta capa
            JwtPayload p = new JwtPayload();
            p.guidUsuario = extraerValorJson(json, "guidUsuario");
            p.guidSesion  = extraerValorJson(json, "guidSesion");
            p.nombre      = extraerValorJson(json, "nombre");
            p.rol         = extraerValorJson(json, "rol");

            string expStr = extraerValorJson(json, "exp");
            if (long.TryParse(expStr, out long exp)) p.exp = exp;

            return p;
        }

        private static string extraerValorJson(string json, string clave) // pasa del jeison a los datos reales ps saca sin el jeison
        {
            string buscar = $"\"{clave}\":";
            int inicio = json.IndexOf(buscar, StringComparison.Ordinal);
            if (inicio < 0) return null;
            inicio += buscar.Length;

            if (json[inicio] == '"')
            {
                inicio++;
                int fin = json.IndexOf('"', inicio);
                return fin < 0 ? null : json.Substring(inicio, fin - inicio);
            }
            else
            {
                int fin = json.IndexOfAny(new[] { ',', '}' }, inicio);
                return fin < 0 ? null : json.Substring(inicio, fin - inicio).Trim();
            }
        }

        private static string firmarHMACSHA256(string datos, string secreto) // usa criptografia para generar la firma
        {
            byte[] key   = Encoding.UTF8.GetBytes(secreto);
            byte[] data  = Encoding.UTF8.GetBytes(datos);
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                byte[] firma = hmac.ComputeHash(data);
                return Base64UrlEncode(firma);
            }
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static byte[] Base64UrlDecode(string input)
        {
            string padded = input.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "=";  break;
            }
            return Convert.FromBase64String(padded);
        }

        // ?????????????????????????????????????????????????????????????????????
        // CORREO
        // ?????????????????????????????????????????????????????????????????????

        /// <summary>Envía correo de verificación con el código de 6 caracteres.</summary>
        public static bool EnviarCorreoVerificacion(string nombre, string apellidos, string correo, string token)
        {
            try
            {
                string cuerpo = $@"
<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 500px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <h2 style='color: #333; text-align: center;'>¡Bienvenido a MiBus!</h2>
        <p style='color: #555; font-size: 16px;'>Hola <strong>{nombre} {apellidos}</strong>,</p>
        <p style='color: #555; font-size: 16px;'>Gracias por registrarte. Para activar tu cuenta, usa el siguiente código de verificación:</p>
        <div style='background: #007bff; color: white; padding: 15px; text-align: center; font-size: 32px; letter-spacing: 8px; border-radius: 5px; margin: 20px 0; font-weight: bold;'>
            {token}
        </div>
        <p style='color: #888; font-size: 14px; text-align: center;'>Este código expira en 24 horas.</p>
        <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
        <p style='color: #aaa; font-size: 12px; text-align: center;'>Si no solicitaste esta verificación, ignora este correo.</p>
    </div>
</body>
</html>";

                using (SmtpClient smtp = new SmtpClient(SMTP_HOST, SMTP_PORT))
                {
                    smtp.EnableSsl             = true;
                    smtp.Credentials           = new System.Net.NetworkCredential(SMTP_USUARIO, SMTP_PASSWORD);
                    smtp.DeliveryMethod        = SmtpDeliveryMethod.Network;

                    MailMessage mensaje = new MailMessage();
                    mensaje.From       = new MailAddress(SMTP_USUARIO, "MiBus App");
                    mensaje.To.Add(new MailAddress(correo, $"{nombre} {apellidos}"));
                    mensaje.Subject    = "Confirma tu cuenta en MiBus";
                    mensaje.Body       = cuerpo;
                    mensaje.IsBodyHtml = true;

                    smtp.Send(mensaje);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ?????????????????????????????????????????????????????????????????????
        // VALIDACIONES COMUNES
        // ?????????????????????????????????????????????????????????????????????

        /// <summary>Valida formato básico de correo electrónico.</summary>
        public static bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return email.Contains("@")
                && email.LastIndexOf("@") == email.IndexOf("@")
                && email.Contains(".")
                && email.IndexOf("@") < email.LastIndexOf(".");
        }
    }

    // ?????????????????????????????????????????????????????????????????????????
    // CLASE AUXILIAR PARA DESERIALIZAR EL PAYLOAD DEL JWT
    // ?????????????????????????????????????????????????????????????????????????
    public class JwtPayload
    {
        public string guidUsuario { get; set; }
        public string guidSesion  { get; set; }
        public string nombre      { get; set; }
        public string rol         { get; set; }
        public long   exp         { get; set; }
    }
}
