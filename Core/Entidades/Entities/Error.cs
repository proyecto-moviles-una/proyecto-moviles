using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades
{
    public class Error
    {
        // se usa para devolver errores en las respuestas de la API
        public int Codigo { get; set; }
        public string Mensaje { get; set; }

    }
}
