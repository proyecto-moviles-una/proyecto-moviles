using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades
{
    public class Empresa
    {
        public Guid?    guidEmpresa   { get; set; }
        public string   nombre        { get; set; }
        public string   telefono      { get; set; }
        public string   correo        { get; set; }
        public bool     estado        { get; set; }
        public DateTime fechaRegistro { get; set; }
    }
}
