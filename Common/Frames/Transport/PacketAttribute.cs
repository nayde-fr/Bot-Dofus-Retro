using System;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Common.Frames.Transport
{
    class PacketAttribute : Attribute
    {
        public PacketAttribute(string identifier) => Identifier = identifier;

        public string Identifier { get; set; }
    }
}
