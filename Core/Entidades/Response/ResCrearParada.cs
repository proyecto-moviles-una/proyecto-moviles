using Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Response
{
    /// <summary>
    /// Respuesta que contiene la información de la parada creada.
    /// </summary>
    public class ResCrearParada : ResBase
    {
        public Parada parada { get; set; }
    }
}
