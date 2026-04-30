using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Request
{
    public class ReqAbrirSesion
    {
        public string token       { get; set; }   // JWT
        public Guid   guidUsuario { get; set; }
        public string origen      { get; set; }   // "MiBus-App"
    }
}
