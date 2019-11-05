using Bot_Dofus_1._29._1.Common.Frames.Transport;
using Bot_Dofus_1._29._1.Common.Network;
using Bot_Dofus_1._29._1.Game.Enums;
using Bot_Dofus_1._29._1.Managers.Accounts;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Common.Frames.Game
{
    internal class ServerSelectionFrame : Frame
    {
        [PacketHandler("HG")]
        public void GameWelcomePacketHandle(TcpClient prmClient, string prmRawPacketData) => prmClient.SendPacket("AT" + prmClient.account.gameTicket);

        [PacketHandler("ATK0")]
        public void CharactersListPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            prmClient.SendPacket("Ak0");
            prmClient.SendPacket("AV");
        }

        [PacketHandler("AV0")]
        public void CharacterSelectPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            prmClient.SendPacket("Ages");
            prmClient.SendPacket("AL");
            prmClient.SendPacket("Af");
        }

        [PacketHandler("ALK")]
        public void CharacterSelectionPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;
            string[] splittedData = prmRawPacketData.Substring(3).Split('|');
            int count = 2;
            bool found = false;
            
            while (count < splittedData.Length && !found)
            {
                string[] _loc11_ = splittedData[count].Split(';');
                int id = int.Parse(_loc11_[0]);
                string characterName = _loc11_[1];

                if (characterName.ToLower().Equals(account.Configuration.CharacterName.ToLower()) || string.IsNullOrEmpty(account.Configuration.CharacterName))
                {
                    prmClient.SendPacket("AS" + id, true);
                    found = true;
                }
                count++;
            }
        }

        //[PacketHandler("BT")]
        //public void get_Tiempo_Servidor(TcpClient cliente, string paquete) => cliente.SendPacket("GI");

        [PacketHandler("GCK")]
        public void ConnectedPacketHandle(TcpClient prmClient, string prmRawPacketData) => prmClient.SendPacket("GI");


        [PacketHandler("ASK")]
        public void SelectedCharacterPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;
            string[] splittedData = prmRawPacketData.Substring(4).Split('|');

            int id = int.Parse(splittedData[0]);
            string characterName = splittedData[1];
            byte characterLevel = byte.Parse(splittedData[2]);
            byte characterClassId = byte.Parse(splittedData[3]);
            byte characterGender = byte.Parse(splittedData[4]);

            account.Game.Character.set_Datos_Personaje(id, characterName, characterLevel, characterGender, characterClassId);
            account.Game.Character.inventario.agregar_Objetos(splittedData[9]);

            prmClient.SendPacket("GC1");

            account.Game.Character.evento_Personaje_Seleccionado();
            account.Game.Character.timer_afk.Change(1200000, 1200000);
            prmClient.account.accountState = AccountState.CONNECTED_INACTIVE;
        }
    }
}
