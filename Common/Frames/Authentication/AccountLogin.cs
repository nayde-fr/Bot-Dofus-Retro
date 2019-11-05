using Bot_Dofus_1._29._1.Common.Frames.Transport;
using Bot_Dofus_1._29._1.Common.Network;
using Bot_Dofus_1._29._1.Game.Enums;
using Bot_Dofus_1._29._1.Game.Server;
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
        [PacketHandler("HC")]
        public void GetWelcomeKeyAsync(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;

            account.accountState = AccountState.CONNECTED;
            account.welcomeKey = prmRawPacketData.Substring(2);

            prmClient.SendPacket("1.30");
            prmClient.SendPacket(prmClient.account.Configuration.Username + "\n" + Hash.Crypt_Password(prmClient.account.Configuration.Password, prmClient.account.welcomeKey));
            prmClient.SendPacket("Af");
        }

        [PacketHandler("Ad")]
        public void GetNickname(TcpClient prmClient, string prmRawPacketData) => prmClient.account.nickname = prmRawPacketData.Substring(2);

        [PacketHandler("Af")]
        public void GetLoginQueue(TcpClient prmClient, string prmRawPacketData) => prmClient.account.logger.log_informacion("File d'attente", "Position " + prmRawPacketData[2] + "/" + prmRawPacketData[4]);

        [PacketHandler("AH")]
        public void GetServerState(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;
            string[] serverList = prmRawPacketData.Substring(2).Split('|');
            GameServer server = account.Game.Server;
            bool firstTime = true;

            foreach(string sv in serverList)
            {
                string[] separator = sv.Split(';');

                int id = int.Parse(separator[0]);
                ServerState serverState = (ServerState)byte.Parse(separator[1]);
                var accountRealm = account.Configuration.GetChosenRealm();

                // Add Method to take name with Id

                if (id == accountRealm.Id)
                {
                    server.RefreshData(id, accountRealm.Name, serverState);
                    account.logger.log_informacion("LOGIN", $"Le serveur {accountRealm.Name} est {account.Game.Server.GetState(serverState)}");

                    if (serverState != ServerState.ONLINE)
                        firstTime = false;
                }
            }

            if(!firstTime && server.serverState == ServerState.ONLINE)
                prmClient.SendPacket("Ax");
        }

        [PacketHandler("AQ")]
        public void GetSecretQuestion(TcpClient prmClient, string prmRawPacketData)
        {
            if (prmClient.account.Game.Server.serverState == ServerState.ONLINE)
                prmClient.SendPacket("Ax", true);
        }

        [PacketHandler("AxK")]
        public void GetServerList(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;
            string[] loc5 = prmRawPacketData.Substring(3).Split('|');
            int counter = 1;
            bool picked = false;

            while (counter < loc5.Length && !picked)
            {
                string[] _loc10_ = loc5[counter].Split(',');
                int serverId = int.Parse(_loc10_[0]);

                if (serverId == account.Game.Server.serverId)
                {
                    if(account.Game.Server.serverState == ServerState.ONLINE)
                    {
                        picked = true;
                        account.Game.Character.evento_Servidor_Seleccionado();
                    }
                    else
                        account.logger.log_Error("LOGIN", "Serveur non accessible lorsque celui-ci se reconnectera");
                }
                counter++;
            }

            if(picked)
                prmClient.SendPacket($"AX{account.Game.Server.serverId}", true);
        }

        [PacketHandler("AXK")]
        public void GetServerSelection(TcpClient prmClient, string prmRawPacketData)
        {
            prmClient.account.gameTicket = prmRawPacketData.Substring(14);
            prmClient.account.SwitchToGameServer(Hash.Decrypt_IP(prmRawPacketData.Substring(3, 8)), Hash.Decrypt_Port(prmRawPacketData.Substring(11, 3).ToCharArray()));
        }
    }
}
