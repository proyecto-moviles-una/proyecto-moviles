using AccesoDatos;
using BCrypt.Net;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Enum.Autenticacion;
using Core.Enum.Favoritos;
using Core.Enum.Generales;
using Core.Enum.Perfil;

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
        public static Error crearError(int codigo) // ste metodo recive un codigo de error
        {
            return new Error
            {
                codigo  = codigo,
                mensaje = obtenerMensajeError(codigo) // y devuelve un objeto con error
            };
        }

        private static string obtenerMensajeError(int codigo)
        {
            switch (codigo)
            {
                case (int)enumErroresAutenticacion.nombreFaltante:              return "El nombre es obligatorio.";
                case (int)enumErroresAutenticacion.apellidosFaltante:           return "Los apellidos son obligatorios.";
                case (int)enumErroresAutenticacion.emailFaltante:               return "El correo electrónico es obligatorio.";
                case (int)enumErroresAutenticacion.emailInvalido:               return "El formato del correo electrónico no es válido.";
                case (int)enumErroresAutenticacion.passwordVacio:               return "La contraseña es obligatoria.";
                case (int)enumErroresAutenticacion.guidDeUsuarioFaltante:       return "El identificador de usuario es obligatorio.";
                case (int)enumErroresAutenticacion.errorActivandoUsuario:       return "Código incorrecto o expirado. Solicite uno nuevo.";
                case (int)enumErroresAutenticacion.loginIncorrecto:             return "Correo o contraseña incorrectos.";
                case (int)enumErroresAutenticacion.usuarioInactivo:             return "La cuenta no ha sido verificada. Revise su correo.";
                case (int)enumErroresAutenticacion.usuarioDesactivado:          return "Esta cuenta ha sido desactivada.";
                case (int)enumErroresAutenticacion.codigoExpirado:              return "El código expiró. Solicite uno nuevo.";
                case (int)enumErroresAutenticacion.cuentaYaActiva:              return "Esta cuenta ya está activa. Puede iniciar sesión.";
                case (int)enumErroresAutenticacion.correoNoRegistrado:          return "El correo no está registrado en el sistema.";
                case (int)enumErroresAutenticacion.correoYaRegistrado:          return "El correo ya está registrado.";
                case (int)enumErroresAutenticacion.guidSesionFaltante:         return "El identificador de sesión es obligatorio.";
                case (int)enumErroresAutenticacion.sesionInvalida:               return "La sesión no es válida o ha expirado.";
                case (int)enumErroresAutenticacion.errorAbrirSesion:             return "No se pudo abrir la sesión.";
                case (int)enumErroresAutenticacion.sesionYaCerrada:              return "La sesión ya fue cerrada anteriormente.";
                case (int)enumErroresAutenticacion.accesoNoAutorizado:           return "No tiene permiso para acceder a este recurso.";
                case (int)enumErroresFavoritos.guidRutaFaltante:                return "El identificador de ruta es obligatorio.";
                case (int)enumErroresFavoritos.favoritoYaExiste:                return "Esta ruta ya está en sus favoritos.";
                case (int)enumErroresFavoritos.favoritoNoExiste:                return "El favorito indicado no existe.";
                case (int)enumErroresFavoritos.guidFavoritoFaltante:            return "El identificador de favorito es obligatorio.";
                case (int)enumErroresPerfil.passwordActualIncorrecto:           return "La contraseña actual es incorrecta.";
                case (int)enumErroresPerfil.passwordNuevoVacio:                 return "La nueva contraseña es obligatoria.";
                case (int)enumErroresPerfil.passwordsNoCoinciden:               return "Las contraseñas no coinciden.";
                case (int)enumErroresPerfil.correoNuevoFaltante:                return "El nuevo correo es obligatorio.";
                case (int)enumErroresPerfil.codigoVerificacionFaltante:         return "El código de verificación es obligatorio.";
                case (int)enumErroresPerfil.codigoVerificacionInvalido:         return "El código de verificación es incorrecto.";
                case (int)enumErroresPerfil.sinSolicitudCambioCorreo:           return "No hay solicitud de cambio de correo pendiente.";
                case (int)enumErroresAutenticacion.usuarioNoDesactivado:        return "La cuenta no está desactivada.";
                case (int)enumErroresGenerales.errorBaseDatos:                  return "Error en base de datos. Intente más tarde.";
                case (int)enumErroresGenerales.errorNoControlado:               return "Ha ocurrido un error inesperado. Contacte al administrador.";
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
                    linq.SP_INSERTAR_BITACORA(
                        req.bitacora.dispositivo,
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
                // Si no hay nombre (reenvio), usar saludo generico
                string saludo = string.IsNullOrEmpty(nombre)
                    ? "Hola,"
                    : string.Format("Hola <strong>{0} {1}</strong>,", nombre, apellidos);

                string cuerpo = string.Format(@"
<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'></head>
<body style='margin:0;padding:0;background-color:#f0f4f8;font-family:""Segoe UI"",Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f0f4f8;padding:40px 20px;'>
    <tr><td align='center'>
      <table width='520' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08);'>

        <!-- Header -->
        <tr>
          <td style='background:linear-gradient(135deg,#1a73e8 0%,#0d47a1 100%);padding:36px 40px;text-align:center;'>
            <div style='display:inline-block;background:rgba(255,255,255,0.15);border-radius:50%;width:56px;height:56px;line-height:56px;font-size:28px;margin-bottom:12px;'>🚌</div>
            <h1 style='margin:0;color:#ffffff;font-size:24px;font-weight:700;letter-spacing:-0.5px;'>MiBus App</h1>
            <p style='margin:6px 0 0;color:rgba(255,255,255,0.8);font-size:13px;'>Transporte público, Alajuela</p>
          </td>
        </tr>

        <!-- Body -->
        <tr>
          <td style='padding:40px 40px 32px;'>
            <h2 style='margin:0 0 8px;color:#1a1a2e;font-size:20px;font-weight:600;'>Verifica tu cuenta</h2>
            <p style='margin:0 0 24px;color:#6b7280;font-size:15px;line-height:1.6;'>{0} ingresa el siguiente código para activar tu cuenta en MiBus:</p>

            <!-- Codigo -->
            <table width='100%' cellpadding='0' cellspacing='0'>
              <tr>
                <td align='center' style='padding:8px 0 28px;'>
                  <table cellpadding='0' cellspacing='0'>
                    <tr>", saludo);

                // Generar celdas individuales para cada caracter del codigo
                System.Text.StringBuilder celdas = new System.Text.StringBuilder();
                foreach (char c in token)
                {
                    celdas.AppendFormat(
                        "<td style='width:48px;height:56px;background:#f8faff;border:2px solid #dbe4ff;" +
                        "border-radius:10px;text-align:center;vertical-align:middle;" +
                        "font-size:26px;font-weight:700;color:#1a73e8;margin:0 4px;" +
                        "font-family:monospace;' align='center'>{0}</td>" +
                        "<td width='8'></td>", c);
                }

                string cuerpo2 = string.Format(@"
                      {0}
                    </tr>
                  </table>
                </td>
              </tr>
            </table>

            <!-- Tiempo de expiracion -->
            <table width='100%' cellpadding='0' cellspacing='0' style='margin-bottom:28px;'>
              <tr>
                <td style='background:#fff8e1;border-left:4px solid #f59e0b;border-radius:0 8px 8px 0;padding:12px 16px;'>
                  <p style='margin:0;color:#92400e;font-size:13px;'>
                    ⏱ <strong>Este código expira en 10 minutos.</strong>
                    Si ya expiró, solicita uno nuevo en la opción <em>Reenviar código</em>.
                  </p>
                </td>
              </tr>
            </table>

            <p style='margin:0;color:#9ca3af;font-size:13px;line-height:1.6;'>
              Si no solicitaste esta verificación, puedes ignorar este mensaje con seguridad.
            </p>
          </td>
        </tr>

        <!-- Footer -->
        <tr>
          <td style='background:#f8fafc;padding:20px 40px;border-top:1px solid #e5e7eb;text-align:center;'>
            <p style='margin:0;color:#9ca3af;font-size:12px;'>
              © 2025 MiBus — Alajuela, Costa Rica<br>
              Este es un mensaje automático, no respondas a este correo.
            </p>
          </td>
        </tr>

      </table>
    </td></tr>
  </table>
</body>
</html>", celdas.ToString());

                string cuerpoCompleto = cuerpo + cuerpo2;

                using (SmtpClient smtp = new SmtpClient(SMTP_HOST, SMTP_PORT))
                {
                    smtp.EnableSsl      = true;
                    smtp.Credentials    = new System.Net.NetworkCredential(SMTP_USUARIO, SMTP_PASSWORD);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    MailMessage mensaje = new MailMessage();
                    mensaje.From       = new MailAddress(SMTP_USUARIO, "MiBus App");
                    mensaje.To.Add(new MailAddress(correo));
                    mensaje.Subject    = "🔐 Tu código de verificación — MiBus";
                    mensaje.Body       = cuerpoCompleto;
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

        /// <summary>Envía el código de verificación al nuevo correo para confirmar el cambio.</summary>
        public static bool EnviarCodigoCambioCorreo(string nombre, string apellidos, string correoNuevo, string codigo)
        {
            try
            {
                string saludo = string.IsNullOrEmpty(nombre)
                    ? "Hola,"
                    : string.Format("Hola <strong>{0} {1}</strong>,", nombre, apellidos);

                string cuerpo = string.Format(@"
<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'></head>
<body style='margin:0;padding:0;background-color:#f0f4f8;font-family:""Segoe UI"",Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f0f4f8;padding:40px 20px;'>
    <tr><td align='center'>
      <table width='520' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08);'>
        <tr>
          <td style='background:linear-gradient(135deg,#1a73e8 0%,#0d47a1 100%);padding:36px 40px;text-align:center;'>
            <h1 style='margin:0;color:#ffffff;font-size:24px;font-weight:700;'>🚌 MiBus App</h1>
            <p style='margin:6px 0 0;color:rgba(255,255,255,0.8);font-size:13px;'>Transporte público, Alajuela</p>
          </td>
        </tr>
        <tr>
          <td style='padding:40px 40px 32px;'>
            <h2 style='margin:0 0 8px;color:#1a1a2e;font-size:20px;font-weight:600;'>Confirma tu nuevo correo</h2>
            <p style='margin:0 0 24px;color:#6b7280;font-size:15px;line-height:1.6;'>{0} usa este código para confirmar el cambio de correo electrónico en MiBus. Expira en <strong>30 minutos</strong>.</p>
            <table width='100%' cellpadding='0' cellspacing='0'>
              <tr>
                <td align='center' style='padding:8px 0 28px;'>
                  <div style='display:inline-block;background:#f0f4ff;border:2px solid #1a73e8;border-radius:12px;padding:18px 40px;'>
                    <span style='font-size:36px;font-weight:800;color:#1a73e8;letter-spacing:10px;font-family:monospace;'>{1}</span>
                  </div>
                </td>
              </tr>
            </table>
            <p style='margin:0;color:#9ca3af;font-size:13px;line-height:1.6;'>
              Si no solicitaste este cambio, ignora este mensaje y tu correo no será modificado.
            </p>
          </td>
        </tr>
        <tr>
          <td style='background:#f8fafc;padding:20px 40px;border-top:1px solid #e5e7eb;text-align:center;'>
            <p style='margin:0;color:#9ca3af;font-size:12px;'>
              © 2025 MiBus — Alajuela, Costa Rica<br>
              Este es un mensaje automático, no respondas a este correo.
            </p>
          </td>
        </tr>
      </table>
    </td></tr>
  </table>
</body>
</html>", saludo, codigo);

                using (SmtpClient smtp = new SmtpClient(SMTP_HOST, SMTP_PORT))
                {
                    smtp.EnableSsl      = true;
                    smtp.Credentials    = new System.Net.NetworkCredential(SMTP_USUARIO, SMTP_PASSWORD);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    MailMessage mensaje = new MailMessage();
                    mensaje.From       = new MailAddress(SMTP_USUARIO, "MiBus App");
                    mensaje.To.Add(new MailAddress(correoNuevo));
                    mensaje.Subject    = "📧 Confirma tu nuevo correo — MiBus";
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

        /// <summary>Envía el código de reactivación al correo del usuario desactivado.</summary>
        public static bool EnviarCodigoReactivacion(string nombre, string apellidos, string correo, string codigo)
        {
            try
            {
                string saludo = string.IsNullOrEmpty(nombre)
                    ? "Hola,"
                    : string.Format("Hola <strong>{0} {1}</strong>,", nombre, apellidos);

                string cuerpo = string.Format(@"
<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'></head>
<body style='margin:0;padding:0;background-color:#f0f4f8;font-family:""Segoe UI"",Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f0f4f8;padding:40px 20px;'>
    <tr><td align='center'>
      <table width='520' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08);'>
        <tr>
          <td style='background:linear-gradient(135deg,#1a73e8 0%,#0d47a1 100%);padding:36px 40px;text-align:center;'>
            <h1 style='margin:0;color:#ffffff;font-size:24px;font-weight:700;'>🚌 MiBus App</h1>
            <p style='margin:6px 0 0;color:rgba(255,255,255,0.8);font-size:13px;'>Transporte público, Alajuela</p>
          </td>
        </tr>
        <tr>
          <td style='padding:40px 40px 32px;'>
            <h2 style='margin:0 0 8px;color:#1a1a2e;font-size:20px;font-weight:600;'>Reactivación de cuenta</h2>
            <p style='margin:0 0 24px;color:#6b7280;font-size:15px;line-height:1.6;'>{0} usa este código para reactivar tu cuenta en MiBus. Expira en <strong>30 minutos</strong>.</p>
            <table width='100%' cellpadding='0' cellspacing='0'>
              <tr>
                <td align='center' style='padding:8px 0 28px;'>
                  <div style='display:inline-block;background:#f0f4ff;border:2px solid #1a73e8;border-radius:12px;padding:18px 40px;'>
                    <span style='font-size:36px;font-weight:800;color:#1a73e8;letter-spacing:10px;font-family:monospace;'>{1}</span>
                  </div>
                </td>
              </tr>
            </table>
            <p style='margin:0;color:#9ca3af;font-size:13px;line-height:1.6;'>
              Si no solicitaste esto, ignora este mensaje.
            </p>
          </td>
        </tr>
        <tr>
          <td style='background:#f8fafc;padding:20px 40px;border-top:1px solid #e5e7eb;text-align:center;'>
            <p style='margin:0;color:#9ca3af;font-size:12px;'>
              © 2025 MiBus — Alajuela, Costa Rica<br>
              Este es un mensaje automático, no respondas a este correo.
            </p>
          </td>
        </tr>
      </table>
    </td></tr>
  </table>
</body>
</html>", saludo, codigo);

                using (SmtpClient smtp = new SmtpClient(SMTP_HOST, SMTP_PORT))
                {
                    smtp.EnableSsl      = true;
                    smtp.Credentials    = new System.Net.NetworkCredential(SMTP_USUARIO, SMTP_PASSWORD);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    MailMessage mensaje = new MailMessage();
                    mensaje.From       = new MailAddress(SMTP_USUARIO, "MiBus App");
                    mensaje.To.Add(new MailAddress(correo));
                    mensaje.Subject    = "🔓 Reactiva tu cuenta — MiBus";
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
