using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades
{
    public class Error
    {
        // se usa para devolver errores en las respuestas de la API,
        // se puede usar el enum de errores para llenar el codigo
        // y mensaje
        public int Codigo { get; set; }
        public string Mensaje { get; set; }
    }
}
