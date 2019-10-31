using System;
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
    public class AccountConfig
    {
        public AccountConfig()
        {
            
        }

        public AccountConfig(string username,string password,int serverId, int realmId, string characterName)
        {
            Username = username;
            Password = password;
            ServerId = serverId;;
            RealmId = realmId;
            CharacterName = characterName;
        }

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int ServerId { get; set; } = 0;

        public int RealmId { get; set; } = 0;

        public string CharacterName { get; set; } = string.Empty;

        public ServerInfo GetChosenServer()
        {
            var serverInfo = ConfigurationManager.Configuration.ServerInfos.FirstOrDefault(s => s.Id == ServerId);
            if (serverInfo == null)
            {
                throw new InvalidOperationException("Invalid Server Info");
            }

            return serverInfo;
        }


        public RealmInfo GetChosenRealm()
        {
            var realmInfo = GetChosenServer().RealmInfos.FirstOrDefault(s => s.Id == RealmId);
            if (realmInfo == null)
            {
                throw new InvalidOperationException("Invalid Realm Info");
            }

            return realmInfo;
        }

    }
}
