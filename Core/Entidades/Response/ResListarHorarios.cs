using System.Collections.Generic;

namespace Core.Entidades.Response
{
    public class ResListarHorarios : ResBase
    {
        public List<Horario> Horarios { get; set; }
    }
}
