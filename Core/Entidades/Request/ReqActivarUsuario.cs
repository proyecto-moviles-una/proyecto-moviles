using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Request
{
    public class ReqActivarUsuario
    {
        public string correo { get; set; }
        public string token  { get; set; }
    }
}
