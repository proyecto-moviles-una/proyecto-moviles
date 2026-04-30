using System;

namespace DTO.Usuario
{
    /// <summary>
    /// DTO recibido en el body del endpoint POST /api/auth/logout
    /// </summary>
    public class DTOCerrarSesion
    {
        public Guid guidSesion { get; set; }
    }
}
