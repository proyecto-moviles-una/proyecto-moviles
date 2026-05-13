using System;

namespace Core.Entidades
{
    public class Horario
    {
        public Guid Guid { get; set; }
        public Guid GuidRuta { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public byte DiasServicio { get; set; }
        public bool Estado { get; set; }
    }
}
