﻿using Bot_Dofus_1._29._1.Common.Frames.Transport;
using Bot_Dofus_1._29._1.Common.Network;
using Bot_Dofus_1._29._1.Game.Enums;
using Bot_Dofus_1._29._1.Game.Server;
using Bot_Dofus_1._29._1.Managers;
using Bot_Dofus_1._29._1.Managers.Accounts;
using Bot_Dofus_1._29._1.Utilities.Crypto;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Common.Frames.Authentication
{
    public class AccountLogin : Frame
    {
        [Packet("HC")]
        public void GetWelcomeKeyAsync(TcpClient prmClient, string prmPacket)
        {
            Account account = prmClient.account;

            account.accountState = AccountState.CONNECTED;
            account.welcomeKey = prmPacket.Substring(2);

            prmClient.SendPacket("1.29");
            prmClient.SendPacket(prmClient.account.accountConfig.accountUsername + "\n" + Hash.Crypt_Password(prmClient.account.accountConfig.accountPassword, prmClient.account.welcomeKey));
            prmClient.SendPacket("Af");
        }

        [Packet("Ad")]
        public void GetNickname(TcpClient prmClient, string prmPacket) => prmClient.account.nickname = prmPacket.Substring(2);

        [Packet("Af")]
        public void GetLoginQueue(TcpClient prmClient, string prmPacket) => prmClient.account.logger.log_informacion("File d'attente", "Position " + prmPacket[2] + "/" + prmPacket[4]);

        [Packet("AH")]
        public void GetServerState(TcpClient prmClient, string prmPacket)
        {
            Account account = prmClient.account;
            string[] serverList = prmPacket.Substring(2).Split('|');
            GameServer server = account.game.Server;
            bool firstTime = true;

            foreach(string sv in serverList)
            {
                string[] separator = sv.Split(';');

                int id = int.Parse(separator[0]);
                ServerState serverState = (ServerState)byte.Parse(separator[1]);
                string serverName = account.accountConfig.server;

                // Add Method to take name with Id

                if (id == account.accountConfig.Get_Server_ID())
                {
                    server.RefreshData(id, serverName, serverState);
                    account.logger.log_informacion("LOGIN", $"Le serveur {serverName} est {account.game.Server.GetState(serverState)}");

                    if (serverState != ServerState.ONLINE)
                        firstTime = false;
                }
            }

            if(!firstTime && server.serverState == ServerState.ONLINE)
                prmClient.SendPacket("Ax");
        }

        [Packet("AQ")]
        public void GetSecretQuestion(TcpClient prmClient, string prmPacket)
        {
            if (prmClient.account.game.Server.serverState == ServerState.ONLINE)
                prmClient.SendPacket("Ax", true);
        }

        [Packet("AxK")]
        public void GetServerList(TcpClient prmClient, string prmPacket)
        {
            Account account = prmClient.account;
            string[] loc5 = prmPacket.Substring(3).Split('|');
            int counter = 1;
            bool picked = false;

            while (counter < loc5.Length && !picked)
            {
                string[] _loc10_ = loc5[counter].Split(',');
                int serverId = int.Parse(_loc10_[0]);

                if (serverId == account.game.Server.serverId)
                {
                    if(account.game.Server.serverState == ServerState.ONLINE)
                    {
                        picked = true;
                        account.game.CharacterClass.evento_Servidor_Seleccionado();
                    }
                    else
                        account.logger.log_Error("LOGIN", "Serveur non accessible lorsque celui-ci se reconnectera");
                }
                counter++;
            }

            if(picked)
                prmClient.SendPacket($"AX{account.game.Server.serverId}", true);
        }

        [Packet("AXK")]
        public void GetServerSelection(TcpClient prmClient, string prmPacket)
        {
            prmClient.account.gameTicket = prmPacket.Substring(14);
            prmClient.account.SwitchToGameServer(Hash.Decrypt_IP(prmPacket.Substring(3, 8)), Hash.Decrypt_Port(prmPacket.Substring(11, 3).ToCharArray()));
        }
    }
}
