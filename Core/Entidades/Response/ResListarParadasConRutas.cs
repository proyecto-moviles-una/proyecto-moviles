using System.Collections.Generic;

namespace Core.Entidades.Response
{
    public class ResListarParadasConRutas : ResBase
    {
        public List<ParadaConRutas> Paradas { get; set; }
    }
}
