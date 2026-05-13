using System;

namespace Core.Entidades.Request
{
    public class ReqCrearHorario
    {
        public Guid GuidRuta { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public byte DiasServicio { get; set; }
    }
}
