using System.Collections.Generic;

namespace Core.Entidades.Response
{
    public class ResListarEmpresas : ResBase
    {
        public List<Empresa> Empresas { get; set; }
    }
}
