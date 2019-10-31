using System.Collections.Generic;
using System.IO;
using System.Linq;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Utilities.Config
{
    public class Configuration
    {
        public List<AccountConfig> Accounts { get; set; } = new List<AccountConfig>();

        public bool DebugPackets { get; set; } = false;
        
        public List<ServerInfo> ServerInfos { get; set; }

    }
}
