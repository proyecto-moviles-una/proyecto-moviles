using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Response
{
    /// <summary>
    /// Respuesta que contiene el listado de paradas.
    /// </summary>
    public class ResListarParadas: ResBase
    {
        public List<Parada> paradas { get; set; }
    }
}
