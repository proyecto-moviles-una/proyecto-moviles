using Core.Entidades.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades.Response
{
    public class ResCrearHistorial: ResBase
    {
        public HistorialViaje historial { get; set; }
    }
}
