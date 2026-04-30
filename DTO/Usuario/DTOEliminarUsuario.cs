using System;

namespace DTO.Usuario
{
    /// <summary>
    /// DTO recibido en el body del endpoint DELETE /api/usuarios/{guid}
    /// </summary>
    public class DTOEliminarUsuario
    {
        public Guid guidUsuario { get; set; }
    }
}
