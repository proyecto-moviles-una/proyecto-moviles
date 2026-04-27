using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Request
{
    public class ReqListarHistorial
    {
        public ReqListarHistorial() { }

        public Guid GuidUsuario { get; set; }
    }
}
