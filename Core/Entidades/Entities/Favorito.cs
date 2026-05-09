using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entidades
{
    public class Favorito
    {
        public Guid?    guidFavorito  { get; set; }
        public Guid?    guidRuta      { get; set; }
        public string   nombreRuta    { get; set; }
        public string   origen        { get; set; }
        public string   destino       { get; set; }
        public decimal? tarifaActual  { get; set; }
        public DateTime fechaRegistro { get; set; }
    }
}
