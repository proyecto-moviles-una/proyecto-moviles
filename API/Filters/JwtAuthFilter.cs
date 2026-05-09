using System.Net;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace API.Filters
{
    /// Es un filtro de seguridad que se ejecuta ANTES del controller,Bloquear cualquier request que no tenga un JWT válido<summary>
    /// Filtro que valida el JWT en el header Authorization: Bearer {token}
    /// Úsalo con [JwtAuth] en cualquier controller o acción que requiera sesión activa.
    /// </summary>
    /// 
    //AQUI REVISA QUE VENGA EL TOKEN SI NO VIEN ERROR 401
    public class JwtAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader == null || authHeader.Scheme != "Bearer" || string.IsNullOrEmpty(authHeader.Parameter))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "Token de autorización requerido. Usa: Authorization: Bearer {tu_token}");
                return;
            }

            Logica.Auth.TokenInfo info = Logica.Auth.JwtHelper.validarToken(authHeader.Parameter);
            // Verifica firma del token, expiracion y formato
            if (!info.valido)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "Token inválido o expirado. Inicia sesión nuevamente.");
                return;
            }

            // Verifica que la sesion siga activa en BD y pertenezca al usuario del token
            // Esto invalida tokens de sesiones cerradas o usuarios desactivados
            if (!Logica.Auth.JwtHelper.validarSesionEnBD(info.guidSesion, info.guidUsuario))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "La sesión no está activa. Inicia sesión nuevamente.");
                return;
            }

            // Guarda los datos del token para usarlos en los controllers
            actionContext.Request.Properties["tokenInfo"] = info;

            base.OnActionExecuting(actionContext);
        }
    }
}


