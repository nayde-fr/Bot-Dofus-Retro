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
        public void get_Combate_Celdas_Posicion(TcpClient cliente, string paquete)
        {
            Account cuenta = cliente.account;
            Map mapa = cuenta.Game.Map;
            string[] _loc3 = paquete.Substring(2).Split('|');

            for (int a = 0; a < _loc3[0].Length; a += 2)
                cuenta.Game.fight.celdas_preparacion.Add(mapa.GetCellFromId((short)((Hash.get_Hash(_loc3[0][a]) << 6) + Hash.get_Hash(_loc3[0][a + 1]))));
                
            if (cuenta.fightExtension.configuracion.desactivar_espectador)
                cliente.SendPacket("fS");

            if (cuenta.canUseMount)
            {
                if (cuenta.fightExtension.configuracion.utilizar_dragopavo && !cuenta.Game.Character.esta_utilizando_dragopavo)
                {
                    cliente.SendPacket("Rr");
                    cuenta.Game.Character.esta_utilizando_dragopavo = true;
                }
            }
        }

        [PacketHandler("GICE")]
        public async Task get_Error_Cambiar_Pos_Pelea(TcpClient cliente, string paquete)
        {
            if(cliente.account.IsFighting())
            {
                await Task.Delay(150);
                cliente.SendPacket("GR1");//boton listo
            }
        }

        [PacketHandler("GIC")]
        public async Task get_Cambiar_Pos_Pelea(TcpClient cliente, string paquete)
        {
            Account cuenta = cliente.account;
            string[] separador_posiciones = paquete.Substring(4).Split('|');
            int id_entidad;
            short celda;
            Map mapa = cuenta.Game.Map;

            foreach (string posicion in separador_posiciones)
            {
                id_entidad = int.Parse(posicion.Split(';')[0]);
                celda = short.Parse(posicion.Split(';')[1]);

                if (id_entidad == cuenta.Game.Character.Id)
                {
                    await Task.Delay(150);
                    cliente.SendPacket("GR1");//boton listo
                }

                Luchadores luchador = cuenta.Game.fight.get_Luchador_Por_Id(id_entidad);
                if (luchador != null)
                    luchador.celda = mapa.GetCellFromId(celda);
            }
        }

        [PacketHandler("GTM")]
        public void get_Combate_Info_Stats(TcpClient cliente, string paquete)
        {
            string[] separador = paquete.Substring(4).Split('|');
            Map mapa = cliente.account.Game.Map;

            for (int i = 0; i < separador.Length; ++i)
            {
                string[] _loc6_ = separador[i].Split(';');
                int id = int.Parse(_loc6_[0]);
                Luchadores luchador = cliente.account.Game.fight.get_Luchador_Por_Id(id);

                if (_loc6_.Length != 0)
                {
                    bool esta_vivo = _loc6_[1].Equals("0");
                    if (esta_vivo)
                    {
                        int vida_actual = int.Parse(_loc6_[2]);
                        byte pa = byte.Parse(_loc6_[3]);
                        byte pm = byte.Parse(_loc6_[4]);
                        short celda = short.Parse(_loc6_[5]);
                        int vida_maxima = int.Parse(_loc6_[7]);

                        if (celda > 0)//son espectadores
                        {
                            byte equipo = Convert.ToByte(id > 0 ? 1 : 0);
                            luchador?.get_Actualizar_Luchador(id, esta_vivo, vida_actual, pa, pm, mapa.GetCellFromId(celda), vida_maxima, equipo);
                        }
                    }
                    else
                        luchador?.get_Actualizar_Luchador(id, esta_vivo, 0, 0, 0, null, 0, 0);
                }
            }
        }

        [PacketHandler("GTR")]
        public void get_Combate_Turno_Listo(TcpClient cliente, string paquete)
        {
            Account cuenta = cliente.account;
            int id = int.Parse(paquete.Substring(3));

            if(cuenta.Game.Character.Id == id)
                cuenta.connexion.SendPacket("BD");

            cuenta.connexion.SendPacket("GT");
        }

        [PacketHandler("GJK")]
        public void get_Combate_Unirse_Pelea(TcpClient cliente, string paquete)
        {
            Account cuenta = cliente.account;

            //GJK - estado|boton_cancelar|mostrat_botones|espectador|tiempo|tipo_pelea
            string[] separador = paquete.Substring(3).Split('|');
            byte estado_pelea = byte.Parse(separador[0]);

            switch (estado_pelea)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    cuenta.Game.fight.get_Combate_Creado();
               break;
            }
        }

        [PacketHandler("GTS")]
        public void get_Combate_Inicio_Turno(TcpClient cliente, string paquete)
        {
            Account cuenta = cliente.account;

            if (int.Parse(paquete.Substring(3).Split('|')[0]) != cuenta.Game.Character.Id || cuenta.Game.fight.total_enemigos_vivos <= 0)
                return;


            cuenta.Game.fight.get_Turno_Iniciado();
        }

        [PacketHandler("GE")]
        public void get_Combate_Finalizado(TcpClient cliente, string paquete)
        {
            Account cuenta = cliente.account;

            cuenta.Game.fight.get_Combate_Acabado();
            cliente.SendPacket("GC1");
        }
    }
}
