using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Request
{
    public class ReqCrearHistorial
    {
        public ReqCrearHistorial() { }

        public Guid GuidUsuario { get; set; }

        public Guid GuidRuta { get; set; }
    }
}
