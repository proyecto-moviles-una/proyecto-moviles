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
            // YA MA A VALIDAR TOKEN DO DE SE VEIRIFICA LA FIRMA DEL TOKEN , EXPERACION Y FORMAT
            if (!info.valido)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "Token inválido o expirado. Inicia sesión nuevamente.");
                return;
            }

            // Disponible en el controller con: Request.Properties["tokenInfo"]
            actionContext.Request.Properties["tokenInfo"] = info; // GUARDA LOS DATOS DEL TOKEN OSEA LOS GUID DE SESION Y USUARIO, NOMBRE

            base.OnActionExecuting(actionContext);
        }
    }
}

