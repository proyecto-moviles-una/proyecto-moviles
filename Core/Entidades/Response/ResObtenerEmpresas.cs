using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Response
{
    public class ResObtenerEmpresas : ResBase
    {
        public List<Empresa> empresas { get; set; }
    }
}
