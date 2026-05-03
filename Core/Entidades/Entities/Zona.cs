using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades
{
    public class Zona
    {
        public Guid?    guidZona      { get; set; }
        public string   nombre        { get; set; }
        public string   descripcion   { get; set; }
        public bool     estado        { get; set; }
        public DateTime fechaRegistro { get; set; }
    }
}
