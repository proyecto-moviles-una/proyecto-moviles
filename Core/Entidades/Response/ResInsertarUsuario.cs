using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Response
{
    public class ResInsertarUsuario : ResBase
    {
        public Guid?   guidUsuario       { get; set; }
        public string  tokenVerificacion { get; set; }   // solo para pruebas — quitar en producción
    }
}
