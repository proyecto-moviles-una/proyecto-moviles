using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enum.Parada
{
    public enum EnumErroresParada
    {
        nombreFaltante = 1,
        descripcionFaltante = 2,
        latitudInvalida = 3,
        longitudInvalida = 4,
        paradaNoEncontrada = 5,
        errorCreandoParada = 6,
        paradaDuplicada = 7,
    }
}
