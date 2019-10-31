using System.Threading.Tasks;
using Bot_Dofus_1._29._1.Common.Frames.Transport;
using Bot_Dofus_1._29._1.Common.Network;
using Bot_Dofus_1._29._1.Game.Character;
using Bot_Dofus_1._29._1.Game.Enums;
using Bot_Dofus_1._29._1.Game.Mapas;
using Bot_Dofus_1._29._1.Game.Mapas.Entidades;
using Bot_Dofus_1._29._1.Managers;
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
        public async Task CharacterMovementsPacketHandle(TcpClient cliente, string rawPacketData)
        {
            Account account = cliente.account;
            string[] separador_jugadores = rawPacketData.Substring(3).Split('|'), informaciones;
            string _loc6, nombre_template, tipo;

            for (int i = 0; i < separador_jugadores.Length; ++i)
            {
                _loc6 = separador_jugadores[i];
                if (_loc6.Length != 0)
                {
                    informaciones = _loc6.Substring(1).Split(';');
                    if (_loc6[0].Equals('+'))
                    {
                        Cell celda = account.Game.Map.GetCellFromId(short.Parse(informaciones[0]));
                        Pelea pelea = account.Game.fight;
                        int id = int.Parse(informaciones[3]);
                        nombre_template = informaciones[4];
                        tipo = informaciones[5];
                        if (tipo.Contains(","))
                            tipo = tipo.Split(',')[0];

                        switch (int.Parse(tipo))
                        {
                            case -1:
                            case -2:
                                if (account.accountState == AccountState.FIGHTING)
                                {
                                    int vida = int.Parse(informaciones[12]);
                                    byte pa = byte.Parse(informaciones[13]);
                                    byte pm = byte.Parse(informaciones[14]);
                                    byte equipo = byte.Parse(informaciones[15]);

                                    pelea.get_Agregar_Luchador(new Luchadores(id, true, vida, pa, pm, celda, vida, equipo));
                                }
                                break;

                            case -3://monstruos
                                string[] templates = nombre_template.Split(',');
                                string[] niveles = informaciones[7].Split(',');

                                Monstruos monstruo = new Monstruos(id, int.Parse(templates[0]), celda, int.Parse(niveles[0]));
                                monstruo.lider_grupo = monstruo;

                                for (int m = 1; m < templates.Length; ++m)
                                    monstruo.moobs_dentro_grupo.Add(new Monstruos(id, int.Parse(templates[m]), celda, int.Parse(niveles[m])));

                                account.Game.Map.entities.TryAdd(id, monstruo);
                                break;

                            case -4://NPC
                                account.Game.Map.entities.TryAdd(id, new Npcs(id, int.Parse(nombre_template), celda));
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
                                        account.Game.Map.entities.TryAdd(id, new Personajes(id, nombre_template, byte.Parse(informaciones[7].ToString()), celda));
                                    else
                                        account.Game.Character.Cell = celda;
                                }
                                else
                                {
                                    int vida = int.Parse(informaciones[14]);
                                    byte pa = byte.Parse(informaciones[15]);
                                    byte pm = byte.Parse(informaciones[16]);
                                    byte equipo = byte.Parse(informaciones[24]);

                                    pelea.get_Agregar_Luchador(new Luchadores(id, true, vida, pa, pm, celda, vida, equipo));

                                    if (account.Game.Character.Id == id && account.fightExtension.configuracion.posicionamiento != PosicionamientoInicioPelea.INMOVIL)
                                    {
                                        await Task.Delay(300);

                                        /** la posicion es aleatoria pero el paquete GP siempre aparecera primero el team donde esta el pj **/
                                        short celda_posicion = pelea.get_Celda_Mas_Cercana_O_Lejana(account.fightExtension.configuracion.posicionamiento == PosicionamientoInicioPelea.CERCA_DE_ENEMIGOS, pelea.celdas_preparacion);
                                        await Task.Delay(300);

                                        if (celda_posicion != celda.cellId)
                                            account.connexion.SendPacket("Gp" + celda_posicion, true);
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
        public void get_Finalizar_Accion(TcpClient cliente, string rawPacketData)
        {
            string[] id_fin_accion = rawPacketData.Substring(3).Split('|');

            cliente.account.connexion.SendPacket("GKK" + id_fin_accion[0]);
        }

        [PacketHandler("GAS")]
        public async Task get_Inicio_Accion(TcpClient cliente, string rawPacketData) => await Task.Delay(200);

        [PacketHandler("GA")]
        public async Task get_Iniciar_Accion(TcpClient cliente, string rawPacketData)
        {
            string[] separador = rawPacketData.Substring(2).Split(';');
            int id_accion = int.Parse(separador[1]);
            Account cuenta = cliente.account;
            Character personaje = cuenta.Game.Character;

            if (id_accion > 0)
            {
                int id_entidad = int.Parse(separador[2]);
                byte tipo_gkk_movimiento;
                Cell celda;
                Luchadores luchador;
                Map mapa = cuenta.Game.Map;
                Pelea pelea = cuenta.Game.fight;

                switch (id_accion)
                {
                    case 1:
                        celda = mapa.GetCellFromId(Hash.Get_Cell_From_Hash(separador[3].Substring(separador[3].Length - 2)));

                        if (!cuenta.IsFighting())
                        {
                            if (id_entidad == personaje.Id && celda.cellId > 0 && personaje.Cell.cellId != celda.cellId)
                            {
                                tipo_gkk_movimiento = byte.Parse(separador[0]);

                                await cuenta.Game.manager.MovementManager.evento_Movimiento_Finalizado(celda, tipo_gkk_movimiento, true);
                            }
                            else if (mapa.entities.TryGetValue(id_entidad, out Entidad entidad))
                            {
                                entidad.Cell = celda;

                                if (ConfigurationManager.Configuration.DebugPackets)
                                    cuenta.logger.log_informacion("DEBUG", "Mouvement détecté d'une entité vers la cellule : " + celda.cellId);
                            }
                            mapa.GetEntitiesRefreshEvent();
                        }
                        else
                        {
                            luchador = pelea.get_Luchador_Por_Id(id_entidad);
                            if (luchador != null)
                            {
                                luchador.celda = celda;

                                if (luchador.id == personaje.Id)
                                {
                                    tipo_gkk_movimiento = byte.Parse(separador[0]);

                                    await Task.Delay(400 + (100 * personaje.Cell.GetDistanceBetweenCells(celda)));
                                    cuenta.connexion.SendPacket("GKK" + tipo_gkk_movimiento);
                                }
                            }
                        }
                        break;

                    case 4:
                        separador = separador[3].Split(',');
                        celda = mapa.GetCellFromId(short.Parse(separador[1]));

                        if (!cuenta.IsFighting() && id_entidad == personaje.Id && celda.cellId > 0 && personaje.Cell.cellId != celda.cellId)
                        {
                            personaje.Cell = celda;
                            await Task.Delay(150);
                            cuenta.connexion.SendPacket("GKK1");
                            mapa.GetEntitiesRefreshEvent();
                            cuenta.Game.manager.MovementManager.movimiento_Actualizado(true);
                        }
                        break;

                    case 5:
                        if (cuenta.IsFighting())
                        {
                            separador = separador[3].Split(',');
                            luchador = pelea.get_Luchador_Por_Id(int.Parse(separador[0]));

                            if (luchador != null)
                                luchador.celda = mapa.GetCellFromId(short.Parse(separador[1]));
                        }
                        break;

                    case 102:
                        if (cuenta.IsFighting())
                        {
                            luchador = pelea.get_Luchador_Por_Id(id_entidad);
                            byte pa_utilizados = byte.Parse(separador[3].Split(',')[1].Substring(1));

                            if (luchador != null)
                                luchador.pa -= pa_utilizados;
                        }
                        break;

                    case 103:
                        if (cuenta.IsFighting())
                        {
                            int id_muerto = int.Parse(separador[3]);

                            luchador = pelea.get_Luchador_Por_Id(id_muerto);
                            if (luchador != null)
                                luchador.esta_vivo = false;
                        }
                        break;

                    case 129: //movimiento en pelea con exito
                        if (cuenta.IsFighting())
                        {
                            luchador = pelea.get_Luchador_Por_Id(id_entidad);
                            byte pm_utilizados = byte.Parse(separador[3].Split(',')[1].Substring(1));

                            if (luchador != null)
                                luchador.pm -= pm_utilizados;

                            if (luchador.id == personaje.Id)
                                pelea.get_Movimiento_Exito(true);
                        }
                        break;

                    case 151://obstaculos invisibles
                        if (cuenta.IsFighting())
                        {
                            luchador = pelea.get_Luchador_Por_Id(id_entidad);

                            if (luchador != null && luchador.id == personaje.Id)
                            {
                                cuenta.logger.log_Error("INFORMATION", "Il n'est pas possible d'effectuer cette action à cause d'un obstacle invisible.");
                                pelea.get_Hechizo_Lanzado(short.Parse(separador[3]), false);
                            }
                        }
                        break;

                    case 181: //efecto de invocacion (pelea)
                        celda = mapa.GetCellFromId(short.Parse(separador[3].Substring(1)));
                        short id_luchador = short.Parse(separador[6]);
                        short vida = short.Parse(separador[15]);
                        byte pa = byte.Parse(separador[16]);
                        byte pm = byte.Parse(separador[17]);
                        byte equipo = byte.Parse(separador[25]);

                        pelea.get_Agregar_Luchador(new Luchadores(id_luchador, true, vida, pa, pm, celda, vida, equipo, id_entidad));
                        break;

                    case 302://fallo critico
                        if (cuenta.IsFighting() && id_entidad == cuenta.Game.Character.Id)
                            pelea.get_Hechizo_Lanzado(0, false);
                        break;

                    case 300: //hechizo lanzado con exito
                        if (cuenta.IsFighting() && id_entidad == cuenta.Game.Character.Id)
                        {
                            short celda_id_lanzado = short.Parse(separador[3].Split(',')[1]);
                            pelea.get_Hechizo_Lanzado(celda_id_lanzado, true);
                        }
                        break;

                    case 501:
                        int tiempo_recoleccion = int.Parse(separador[3].Split(',')[1]);
                        celda = mapa.GetCellFromId(short.Parse(separador[3].Split(',')[0]));
                        byte tipo_gkk_recoleccion = byte.Parse(separador[0]);

                        await cuenta.Game.manager.GatheringManager.evento_Recoleccion_Iniciada(id_entidad, tiempo_recoleccion, celda.cellId, tipo_gkk_recoleccion);
                        break;

                    case 900:
                        cuenta.connexion.SendPacket("GA902" + id_entidad, true);
                        cuenta.logger.log_informacion("INFORMATION", "Le défi avec le personnage ID : " + id_entidad + " est annulée");
                        break;
                }
            }
        }

        [PacketHandler("GDF")]
        public void InteractiveStatePacketHandle(TcpClient cliente, string rawPacketData)
        {
            foreach (string interactivo in rawPacketData.Substring(4).Split('|'))
            {
                string[] separador = interactivo.Split(';');
                Account cuenta = cliente.account;
                short celda_id = short.Parse(separador[0]);
                byte estado = byte.Parse(separador[1]);

                switch (estado)
                {
                    case 2:
                        cuenta.Game.Map.interactives[celda_id].IsUsable = false;
                        break;

                    case 3:
                        if (cuenta.Game.Map.interactives.TryGetValue(celda_id, out var value))
                            value.IsUsable = false;

                        if (cuenta.IsGathering())
                            cuenta.Game.manager.GatheringManager.evento_Recoleccion_Acabada(GatheringResult.OK, celda_id);
                        else
                            cuenta.Game.manager.GatheringManager.evento_Recoleccion_Acabada(GatheringResult.STOLEN, celda_id);
                        break;

                    case 4:// reaparece asi se fuerza el cambio de mapa 
                        cuenta.Game.Map.interactives[celda_id].IsUsable = false;
                        break;
                }
            }
        }

        [PacketHandler("GDM")]
        public void NewMapPacketHandle(TcpClient cliente, string rawPacketData)
        {
            if (rawPacketData.Length == 21)
            {
                // Required in Amakna
                string[] _loc3 = rawPacketData.Split('|');
                var mapId = int.Parse(_loc3[1]);
                cliente.SendPacket($"GDm{mapId}");
            }
            else
            {
                cliente.account.Game.Map.GetRefreshMap(rawPacketData.Substring(4));
                cliente.SendPacket("GI"); // Amakna only ?
            }
        }

        [PacketHandler("GDK")]
        public void ChangeMapPacketHandle(TcpClient cliente, string rawPacketData) => cliente.account.Game.Map.GetMapRefreshEvent();

        [PacketHandler("GV")]
        public void ChangeScreenPacketHandle(TcpClient cliente, string rawPacketData) => cliente.account.connexion.SendPacket("GC1");
    }
}
