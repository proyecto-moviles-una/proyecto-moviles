using System.Collections.Generic;

namespace Core.Entidades
{
    public class ParadaConRutas : Parada
    {
        public List<Ruta> Rutas { get; set; } = new List<Ruta>();
    }
}
