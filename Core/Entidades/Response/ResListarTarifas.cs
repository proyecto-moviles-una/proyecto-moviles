using System.Collections.Generic;

namespace Core.Entidades.Response
{
    public class ResListarTarifas : ResBase
    {
        public List<Tarifa> Tarifas { get; set; }
    }
}
