using System.Collections.Generic;
using Core.Entidades;

namespace Core.Entidades.Response
{
    public class ResListarRutaParadaAsociaciones : ResBase
    {
        public List<RutaParadaAsociacion> Asociaciones { get; set; }
    }
}
