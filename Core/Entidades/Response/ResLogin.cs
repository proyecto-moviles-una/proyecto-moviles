using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Response
{
    public class ResLogin : ResBase
    {
        public Usuario usuario    { get; set; }
        public Guid?   guidSesion { get; set; }
        public string  token      { get; set; }   // JWT firmado con HMACSHA256
    }
}
