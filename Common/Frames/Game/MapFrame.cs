using System.Threading.Tasks;
using Bot_Dofus_1._29._1.Common.Frames.Transport;
using Bot_Dofus_1._29._1.Common.Network;
using Bot_Dofus_1._29._1.Game.Enums;
using Bot_Dofus_1._29._1.Game.Mapas;
using Bot_Dofus_1._29._1.Game.Mapas.Entidades;
using Bot_Dofus_1._29._1.Managers.Accounts;
using Bot_Dofus_1._29._1.Managers.Characters;
using Bot_Dofus_1._29._1.Managers.Fights;
using Bot_Dofus_1._29._1.Managers.Fights.Enums;
using Bot_Dofus_1._29._1.Managers.Fights.Peleadores;
using Bot_Dofus_1._29._1.Managers.Jobs;
using Bot_Dofus_1._29._1.Utilities.Config;
using Bot_Dofus_1._29._1.Utilities.Crypto;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Common.Frames.Game
{
    internal class MapFrame : Frame
    {
        [PacketHandler("GM")]
        public async Task CharacterMovementsPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;
            string[] playersSplit = prmRawPacketData.Substring(3).Split('|'), infos;
            string _loc6, templateNumber, type;

            for (int i = 0; i < playersSplit.Length; ++i)
            {
                _loc6 = playersSplit[i];
                if (_loc6.Length != 0)
                {
                    infos = _loc6.Substring(1).Split(';');
                    if (_loc6[0].Equals('+'))
                    {
                        Cell cell = account.Game.Map.GetCellFromId(short.Parse(infos[0]));
                        Pelea fight = account.Game.fight;
                        int id = int.Parse(infos[3]);
                        templateNumber = infos[4];
                        type = infos[5];
                        if (type.Contains(","))
                            type = type.Split(',')[0];

                        switch (int.Parse(type))
                        {
                            case -1:
                            case -2:
                                if (account.accountState == AccountState.FIGHTING)
                                {
                                    int hp = int.Parse(infos[12]);
                                    byte ap = byte.Parse(infos[13]);
                                    byte mp = byte.Parse(infos[14]);
                                    byte team = byte.Parse(infos[15]);

                                    fight.get_Agregar_Luchador(new Luchadores(id, true, hp, ap, mp, cell, hp, team));
                                }
                                break;

                            case -3://monstruos
                                string[] templates = templateNumber.Split(',');
                                string[] levels = infos[7].Split(',');

                                Monstruos monster = new Monstruos(id, int.Parse(templates[0]), cell, int.Parse(levels[0]));
                                monster.lider_grupo = monster;

                                for (int m = 1; m < templates.Length; ++m)
                                    monster.moobs_dentro_grupo.Add(new Monstruos(id, int.Parse(templates[m]), cell, int.Parse(levels[m])));

                                account.Game.Map.entities.TryAdd(id, monster);
                                break;

                            case -4://NPC
                                account.Game.Map.entities.TryAdd(id, new Npcs(id, int.Parse(templateNumber), cell));
                                break;

                            case -5:
                            case -6:
                            case -7:
                            case -8:
                            case -9:
                            case -10:
                                break;

                            default:// jugador
                                if (account.accountState != AccountState.FIGHTING)
                                {
                                    if (account.Game.Character.Id != id)
                                        account.Game.Map.entities.TryAdd(id, new Personajes(id, templateNumber, byte.Parse(infos[7].ToString()), cell));
                                    else
                                        account.Game.Character.Cell = cell;
                                }
                                else
                                {
                                    int hp = int.Parse(infos[14]);
                                    byte ap = byte.Parse(infos[15]);
                                    byte mp = byte.Parse(infos[16]);
                                    byte team = byte.Parse(infos[24]);

                                    fight.get_Agregar_Luchador(new Luchadores(id, true, hp, ap, mp, cell, hp, team));

                                    if (account.Game.Character.Id == id && account.fightExtension.configuracion.posicionamiento != PosicionamientoInicioPelea.INMOVIL)
                                    {
                                        await Task.Delay(300);

                                        /** la posicion es aleatoria pero el paquete GP siempre aparecera primero el team donde esta el pj **/
                                        short cellPosition = fight.get_Celda_Mas_Cercana_O_Lejana(account.fightExtension.configuracion.posicionamiento == PosicionamientoInicioPelea.CERCA_DE_ENEMIGOS, fight.celdas_preparacion);
                                        await Task.Delay(300);

                                        if (cellPosition != cell.cellId)
                                            account.connexion.SendPacket("Gp" + cellPosition, true);
                                        else
                                            account.connexion.SendPacket("GR1");
                                    }
                                    else if (account.Game.Character.Id == id)
                                    {
                                        await Task.Delay(300);
                                        account.connexion.SendPacket("GR1");//boton listo
                                    }
                                }
                                break;
                        }
                    }
                    else if (_loc6[0].Equals('-'))
                    {
                        if (account.accountState != AccountState.FIGHTING)
                        {
                            int id = int.Parse(_loc6.Substring(1));
                            account.Game.Map.entities.TryRemove(id, out Entidad entidad);
                        }
                    }
                }
            }
        }

        [PacketHandler("GAF")]
        public void EndActionPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            string[] idEndAction = prmRawPacketData.Substring(3).Split('|');

            prmClient.account.connexion.SendPacket("GKK" + idEndAction[0]);
        }

        [PacketHandler("GAS")]
        public async Task InitialActionPacketHandle(TcpClient prmClient, string prmRawPacketData) => await Task.Delay(200);

        [PacketHandler("GA")]
        public async Task StartActionPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            string[] splittedData = prmRawPacketData.Substring(2).Split(';');
            int actionId = int.Parse(splittedData[1]);
            Account account = prmClient.account;
            Character character = account.Game.Character;

            if (actionId > 0)
            {
                int entityId = int.Parse(splittedData[2]);
                byte gttMovementType;
                Cell cell;
                Luchadores fighter;
                Map map = account.Game.Map;
                Pelea fight = account.Game.fight;

                switch (actionId)
                {
                    case 1:
                        cell = map.GetCellFromId(Hash.Get_Cell_From_Hash(splittedData[3].Substring(splittedData[3].Length - 2)));

                        if (!account.IsFighting())
                        {
                            if (entityId == character.Id && cell.cellId > 0 && character.Cell.cellId != cell.cellId)
                            {
                                gttMovementType = byte.Parse(splittedData[0]);
                                await account.Game.manager.MovementManager.evento_Movimiento_Finalizado(cell, gttMovementType, true);
                            }
                            else if (map.entities.TryGetValue(entityId, out Entidad entidad))
                            {
                                entidad.Cell = cell;
                                if (ConfigurationManager.Configuration.DebugPackets)
                                    account.logger.log_informacion("DEBUG", "Mouvement détecté d'une entité vers la cellule : " + cell.cellId);
                            }
                            map.GetEntitiesRefreshEvent();
                        }
                        else
                        {
                            fighter = fight.get_Luchador_Por_Id(entityId);
                            if (fighter != null)
                            {
                                fighter.celda = cell;
                                if (fighter.id == character.Id)
                                {
                                    gttMovementType = byte.Parse(splittedData[0]);
                                    await Task.Delay(400 + (100 * character.Cell.GetDistanceBetweenCells(cell)));
                                    account.connexion.SendPacket("GKK" + gttMovementType);
                                }
                            }
                        }
                        break;

                    case 4:
                        splittedData = splittedData[3].Split(',');
                        cell = map.GetCellFromId(short.Parse(splittedData[1]));

                        if (!account.IsFighting() && entityId == character.Id && cell.cellId > 0 && character.Cell.cellId != cell.cellId)
                        {
                            character.Cell = cell;
                            await Task.Delay(150);
                            account.connexion.SendPacket("GKK1");
                            map.GetEntitiesRefreshEvent();
                            account.Game.manager.MovementManager.movimiento_Actualizado(true);
                        }
                        break;

                    case 5:
                        if (account.IsFighting())
                        {
                            splittedData = splittedData[3].Split(',');
                            fighter = fight.get_Luchador_Por_Id(int.Parse(splittedData[0]));

                            if (fighter != null)
                                fighter.celda = map.GetCellFromId(short.Parse(splittedData[1]));
                        }
                        break;

                    case 102:
                        if (account.IsFighting())
                        {
                            fighter = fight.get_Luchador_Por_Id(entityId);
                            byte usedAp = byte.Parse(splittedData[3].Split(',')[1].Substring(1));

                            if (fighter != null)
                                fighter.pa -= usedAp;
                        }
                        break;

                    case 103:
                        if (account.IsFighting())
                        {
                            int deadId = int.Parse(splittedData[3]);

                            fighter = fight.get_Luchador_Por_Id(deadId);
                            if (fighter != null)
                                fighter.esta_vivo = false;
                        }
                        break;

                    case 129: //movimiento en pelea con exito
                        if (account.IsFighting())
                        {
                            fighter = fight.get_Luchador_Por_Id(entityId);
                            byte usedMp = byte.Parse(splittedData[3].Split(',')[1].Substring(1));

                            if (fighter != null)
                                fighter.pm -= usedMp;

                            if (fighter.id == character.Id)
                                fight.get_Movimiento_Exito(true);
                        }
                        break;

                    case 151://obstaculos invisibles
                        if (account.IsFighting())
                        {
                            fighter = fight.get_Luchador_Por_Id(entityId);

                            if (fighter != null && fighter.id == character.Id)
                            {
                                account.logger.log_Error("INFORMATION", "Il n'est pas possible d'effectuer cette action à cause d'un obstacle invisible.");
                                fight.get_Hechizo_Lanzado(short.Parse(splittedData[3]), false);
                            }
                        }
                        break;

                    case 181: //efecto de invocacion (pelea)
                        cell = map.GetCellFromId(short.Parse(splittedData[3].Substring(1)));
                        short fighterId = short.Parse(splittedData[6]);
                        short hp = short.Parse(splittedData[15]);
                        byte ap = byte.Parse(splittedData[16]);
                        byte mp = byte.Parse(splittedData[17]);
                        byte team = byte.Parse(splittedData[25]);

                        fight.get_Agregar_Luchador(new Luchadores(fighterId, true, hp, ap, mp, cell, hp, team, entityId));
                        break;

                    case 302://fallo critico
                        if (account.IsFighting() && entityId == account.Game.Character.Id)
                            fight.get_Hechizo_Lanzado(0, false);
                        break;

                    case 300: //hechizo lanzado con exito
                        if (account.IsFighting() && entityId == account.Game.Character.Id)
                        {
                            short targetCellId = short.Parse(splittedData[3].Split(',')[1]);
                            fight.get_Hechizo_Lanzado(targetCellId, true);
                        }
                        break;

                    case 501:
                        int collectionTime = int.Parse(splittedData[3].Split(',')[1]);
                        cell = map.GetCellFromId(short.Parse(splittedData[3].Split(',')[0]));
                        byte gkkCollectionType = byte.Parse(splittedData[0]);

                        await account.Game.manager.GatheringManager.evento_Recoleccion_Iniciada(entityId, collectionTime, cell.cellId, gkkCollectionType);
                        break;

                    case 900:
                        account.connexion.SendPacket("GA902" + entityId, true);
                        account.logger.log_informacion("INFORMATION", "Le défi avec le personnage ID : " + entityId + " est annulée");
                        break;
                }
            }
        }

        [PacketHandler("GDF")]
        public void InteractiveStatePacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            foreach (string interactive in prmRawPacketData.Substring(4).Split('|'))
            {
                string[] splitter = interactive.Split(';');
                Account account = prmClient.account;
                short cellId = short.Parse(splitter[0]);
                byte state = byte.Parse(splitter[1]);

                switch (state)
                {
                    case 2:
                        account.Game.Map.interactives[cellId].IsUsable = false;
                        break;

                    case 3:
                        if (account.Game.Map.interactives.TryGetValue(cellId, out var value))
                            value.IsUsable = false;

                        if (account.IsGathering())
                            account.Game.manager.GatheringManager.evento_Recoleccion_Acabada(GatheringResult.OK, cellId);
                        else
                            account.Game.manager.GatheringManager.evento_Recoleccion_Acabada(GatheringResult.STOLEN, cellId);
                        break;

                    case 4:// reaparece asi se fuerza el cambio de mapa 
                        account.Game.Map.interactives[cellId].IsUsable = false;
                        break;
                }
            }
        }

        [PacketHandler("GDM")]
        public void NewMapPacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            if (prmRawPacketData.Length == 21)
            {
                // Required in Amakna
                string[] _loc3 = prmRawPacketData.Split('|');
                var mapId = int.Parse(_loc3[1]);
                prmClient.SendPacket($"GDm{mapId}");
            }
            else
            {
                prmClient.account.Game.Map.GetRefreshMap(prmRawPacketData.Substring(4));
                prmClient.SendPacket("GI"); // Amakna only ?
            }
        }

        [PacketHandler("GDK")]
        public void ChangeMapPacketHandle(TcpClient prmClient, string prmRawPacketData) => prmClient.account.Game.Map.GetMapRefreshEvent();

        [PacketHandler("GV")]
        public void ChangeScreenPacketHandle(TcpClient prmClient, string prmRawPacketData) => prmClient.account.connexion.SendPacket("GC1");
    }
}
