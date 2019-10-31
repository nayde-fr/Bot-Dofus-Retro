using System;

namespace Bot_Dofus_1._29._1.Game.Mapas.Entidades
{
    public interface Entidad : IDisposable
    {
        int Id { get; set; }
        Cell Cell { get; set; }
    }
}