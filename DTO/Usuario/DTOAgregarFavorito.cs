using System;

namespace DTO.Usuario
{
    /// <summary>
    /// DTO recibido en el body del endpoint POST /api/favoritos/{guidUsuario}
    /// </summary>
    public class DTOAgregarFavorito
    {
        public Guid guidRuta { get; set; }
    }
}
