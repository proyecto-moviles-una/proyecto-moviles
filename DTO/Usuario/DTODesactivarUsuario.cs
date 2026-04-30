using System;

namespace DTO.Usuario
{
    /// <summary>
    /// DTO recibido en el body del endpoint PUT /api/usuarios/{guid}/desactivar
    /// </summary>
    public class DTODesactivarUsuario
    {
        public Guid guidUsuario { get; set; }
    }
}
