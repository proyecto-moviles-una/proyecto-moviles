using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Request
{
    public class ReqActualizarUsuario
    {
        public Guid   guidUsuario { get; set; }
        public string nombre      { get; set; }
        public string apellidos   { get; set; }
    }
}
