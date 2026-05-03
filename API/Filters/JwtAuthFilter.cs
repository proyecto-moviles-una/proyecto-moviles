using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace API.Filters
{
    /// Es un filtro de seguridad que se ejecuta ANTES del controller,Bloquear cualquier request que no tenga un JWT v�lido<summary>
    /// Filtro que valida el JWT en el header Authorization: Bearer {token}
    /// �salo con [JwtAuth] en cualquier controller o acci�n que requiera sesi�n activa.
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
                    "Token de autorizaci�n requerido. Usa: Authorization: Bearer {tu_token}");
                return;
            }

            Logica.Auth.TokenInfo info = Logica.Auth.JwtHelper.validarToken(authHeader.Parameter);
            Debug.WriteLine("[JwtAuth] Token recibido. Válido=" + info.valido + " guidSesion=" + info.guidSesion);

            if (!info.valido)
            {
                Debug.WriteLine("[JwtAuth] Token inválido o expirado.");
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "Token inv�lido o expirado. Inicia sesi�n nuevamente.");
                return;
            }

            bool sesionActiva = Logica.Auth.JwtHelper.validarSesionEnBD(info.guidSesion, info.guidUsuario);
            Debug.WriteLine("[JwtAuth] Sesión activa en BD=" + sesionActiva);

            if (!sesionActiva)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "La sesi�n no est� activa. Inicia sesi�n nuevamente.");
                return;
            }

            // Guarda los datos del token para usarlos en los controllers
            actionContext.Request.Properties["tokenInfo"] = info;

            base.OnActionExecuting(actionContext);
        }
    }
}


