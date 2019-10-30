using System.Reflection;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1
    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Common.Frames.Transport
{
    public class PacketHandler
    {
        public object Instance { get; set; }
        public string PacketIdentifier { get; set; }
        public MethodInfo Information { get; set; }

        public PacketHandler(object instance, string identifier, MethodInfo information)
        {
            Instance = instance;
            PacketIdentifier = identifier;
            Information = information;
        }
    }
}