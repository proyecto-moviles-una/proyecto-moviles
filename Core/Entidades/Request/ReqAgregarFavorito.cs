using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Request
{
    public class ReqAgregarFavorito
    {
        public Guid guidUsuario { get; set; }
        public Guid guidRuta    { get; set; }
    }
}
