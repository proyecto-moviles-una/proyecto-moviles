using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades
{
    public class Usuario
    {
        public Guid?   guid      { get; set; }
        public string  nombre    { get; set; }
        public string  apellidos { get; set; }
        public string  email     { get; set; }
        public string  password  { get; set; }
        public int?    estado    { get; set; }
    }
}
