using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Request
{
    public class ReqInsertarEmpresa
    {
        public string nombre   { get; set; }
        public string telefono { get; set; }
        public string correo   { get; set; }
    }
}
