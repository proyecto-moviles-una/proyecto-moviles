using Core.Enum;

namespace Core.Entidades
{
    public class Bitacora
    {
        public string       clase       { get; set; }
        public string       metodo      { get; set; }
        public enumBitacora tipo        { get; set; }
        public int          codigoError { get; set; }
        public string       descripcion { get; set; }
        public string       request     { get; set; }
        public string       response    { get; set; }
        public string       dispositivo { get; set; }
    }
}
