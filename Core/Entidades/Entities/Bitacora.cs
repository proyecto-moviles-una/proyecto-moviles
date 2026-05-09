using Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades
{
    public class Bitacora
    {
        public string       dispositivo { get; set; }
        public string       clase       { get; set; }
        public string       metodo      { get; set; }
        public enumBitacora tipo        { get; set; }
        public int          errorId     { get; set; }
        public string       descripcion { get; set; }
        public string       request     { get; set; }
        public string       response    { get; set; }
    }
}
