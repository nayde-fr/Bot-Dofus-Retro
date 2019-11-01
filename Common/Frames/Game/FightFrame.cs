using System;
using System.Threading.Tasks;
using Bot_Dofus_1._29._1.Common.Frames.Transport;
using Bot_Dofus_1._29._1.Common.Network;
using Bot_Dofus_1._29._1.Game.Mapas;
using Bot_Dofus_1._29._1.Managers;
using Bot_Dofus_1._29._1.Managers.Accounts;
using Bot_Dofus_1._29._1.Managers.Fights.Peleadores;
using Bot_Dofus_1._29._1.Utilities.Crypto;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Common.Frames.Game
{
    internal class FightFrame : Frame
    {
        [PacketHandler("GP")]
        public void FightCellsPositionsPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;
            Map map = account.Game.Map;
            string[] splittedData = prmRawPacketData.Substring(2).Split('|');

            for (int a = 0; a < splittedData[0].Length; a += 2)
                account.Game.fight.celdas_preparacion.Add(map.GetCellFromId((short)((Hash.get_Hash(splittedData[0][a]) << 6) + Hash.get_Hash(splittedData[0][a + 1]))));
                
            if (account.fightExtension.configuracion.desactivar_espectador)
                prmClient.SendPacket("fS");

            if (account.canUseMount)
            {
                if (account.fightExtension.configuracion.utilizar_dragopavo && !account.Game.Character.esta_utilizando_dragopavo)
                {
                    prmClient.SendPacket("Rr");
                    account.Game.Character.esta_utilizando_dragopavo = true;
                }
            }
        }

        [PacketHandler("GICE")]
        public async Task FightChangePosErrorPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            if(prmClient.account.IsFighting())
            {
                await Task.Delay(150);
                prmClient.SendPacket("GR1");//boton listo
            }
        }

        [PacketHandler("GIC")]
        public async Task FightChangePosPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;
            string[] positionsSplitter = prmRawPacketData.Substring(4).Split('|');
            int entityId;
            short cell;
            Map map = account.Game.Map;

            foreach (string position in positionsSplitter)
            {
                entityId = int.Parse(position.Split(';')[0]);
                cell = short.Parse(position.Split(';')[1]);

                if (entityId == account.Game.Character.Id)
                {
                    await Task.Delay(150);
                    prmClient.SendPacket("GR1");//boton listo
                }

                Luchadores fighter = account.Game.fight.get_Luchador_Por_Id(entityId);
                if (fighter != null)
                    fighter.celda = map.GetCellFromId(cell);
            }
        }

        [PacketHandler("GTM")]
        public void FightStatsInfosPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            string[] splittedData = prmRawPacketData.Substring(4).Split('|');
            Map map = prmClient.account.Game.Map;

            for (int i = 0; i < splittedData.Length; ++i)
            {
                string[] _loc6_ = splittedData[i].Split(';');
                int id = int.Parse(_loc6_[0]);
                Luchadores fighter = prmClient.account.Game.fight.get_Luchador_Por_Id(id);

                if (_loc6_.Length != 0)
                {
                    bool isAlive = _loc6_[1].Equals("0");
                    if (isAlive)
                    {
                        int currentHp = int.Parse(_loc6_[2]);
                        byte ap = byte.Parse(_loc6_[3]);
                        byte mp = byte.Parse(_loc6_[4]);
                        short cell = short.Parse(_loc6_[5]);
                        int maxHp = int.Parse(_loc6_[7]);

                        if (cell > 0)//spectators
                        {
                            byte team = Convert.ToByte(id > 0 ? 1 : 0);
                            fighter?.get_Actualizar_Luchador(id, isAlive, currentHp, ap, mp, map.GetCellFromId(cell), maxHp, team);
                        }
                    }
                    else
                        fighter?.get_Actualizar_Luchador(id, isAlive, 0, 0, 0, null, 0, 0);
                }
            }
        }

        [PacketHandler("GTR")]
        public void FightTurnReadyPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;
            int id = int.Parse(prmRawPacketData.Substring(3));

            if(account.Game.Character.Id == id)
                account.connexion.SendPacket("BD");

            account.connexion.SendPacket("GT");
        }

        [PacketHandler("GJK")]
        public void FightJoinFightPacketHandle(TcpClient prmClient, string prmSocket)
        {
            Account account = prmClient.account;

            //GJK - estado|boton_cancelar|mostrat_botones|espectador|tiempo|tipo_pelea
            string[] splittedData = prmSocket.Substring(3).Split('|');
            byte fightingState = byte.Parse(splittedData[0]);

            switch (fightingState)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    account.Game.fight.get_Combate_Creado();
               break;
            }
        }

        [PacketHandler("GTS")]
        public void FightStartTurnPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;

            if (int.Parse(prmRawPacketData.Substring(3).Split('|')[0]) != account.Game.Character.Id || account.Game.fight.total_enemigos_vivos <= 0)
                return;

            account.Game.fight.get_Turno_Iniciado();
        }

        [PacketHandler("GE")]
        public void FightFinalizedPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;

            account.Game.fight.get_Combate_Acabado();
            prmClient.SendPacket("GC1");
        }
    }
}
