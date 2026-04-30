using System.Collections.Generic;

namespace Core.Entidades.Response
{
    public class ResObtenerListaUsuarios : ResBase
    {
        public List<Usuario> usuarios { get; set; }
    }
}
