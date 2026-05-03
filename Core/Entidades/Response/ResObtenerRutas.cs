using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Response
{
    public class ResObtenerRutas : ResBase
    {
        public List<Ruta> rutas { get; set; }
    }
}