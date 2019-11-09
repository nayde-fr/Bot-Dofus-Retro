/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Game.Mapas.Entidades
{
    public class Personajes : Entidad
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        public byte sexo { get; set; }
        public Cell Cell { get; set; }
        private bool disposed;

        public Personajes(int _id, string _nombre_personaje, byte _sexo, Cell _celda)
        {
            Id = _id;
            nombre = _nombre_personaje;
            sexo = _sexo;
            Cell = _celda;
        }

        #region Zona Dispose
        ~Personajes() => Dispose(false);
        public void Dispose() => Dispose(true);

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Cell = null;
                disposed = true;
            }
        }
        #endregion
    }
}
