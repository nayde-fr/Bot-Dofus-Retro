using Bot_Dofus_1._29._1.Common.Frames.Transport;
using Bot_Dofus_1._29._1.Common.Network;
using Bot_Dofus_1._29._1.Game.Enums;
using Bot_Dofus_1._29._1.Managers;
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
        public void bienvenida_Juego(TcpClient cliente, string paquete) => cliente.SendPacket("AT" + cliente.account.gameTicket);

        [PacketHandler("ATK0")]
        public void resultado_Servidor_Seleccion(TcpClient cliente, string paquete)
        {
            cliente.SendPacket("Ak0");
            cliente.SendPacket("AV");
        }

        [PacketHandler("AV0")]
        public void lista_Personajes(TcpClient cliente, string paquete)
        {
            cliente.SendPacket("Ages");
            cliente.SendPacket("AL");
            cliente.SendPacket("Af");
        }

        [PacketHandler("ALK")]
        public void seleccionar_Personaje(TcpClient cliente, string paquete)
        {
            Account cuenta = cliente.account;
            string[] _loc6_ = paquete.Substring(3).Split('|');
            int contador = 2;
            bool encontrado = false;

            while (contador < _loc6_.Length && !encontrado)
            {
                string[] _loc11_ = _loc6_[contador].Split(';');
                int id = int.Parse(_loc11_[0]);
                string nombre = _loc11_[1];

                if (nombre.ToLower().Equals(cuenta.Configuration.CharacterName.ToLower()) || string.IsNullOrEmpty(cuenta.Configuration.CharacterName))
                {
                    cliente.SendPacket("AS" + id, true);
                    encontrado = true;
                }

                contador++;
            }
        }

        //[PacketHandler("BT")]
        //public void get_Tiempo_Servidor(TcpClient cliente, string paquete) => cliente.SendPacket("GI");

        [PacketHandler("GCK")]
        public void ConnectedPacketHandler(TcpClient cliente, string paquete) => cliente.SendPacket("GI");


        [PacketHandler("ASK")]
        public void personaje_Seleccionado(TcpClient cliente, string paquete)
        {
            Account cuenta = cliente.account;
            string[] _loc4 = paquete.Substring(4).Split('|');

            int id = int.Parse(_loc4[0]);
            string nombre = _loc4[1];
            byte nivel = byte.Parse(_loc4[2]);
            byte raza_id = byte.Parse(_loc4[3]);
            byte sexo = byte.Parse(_loc4[4]);

            cuenta.Game.Character.set_Datos_Personaje(id, nombre, nivel, sexo, raza_id);
            cuenta.Game.Character.inventario.agregar_Objetos(_loc4[9]);

            cliente.SendPacket("GC1");

            cuenta.Game.Character.evento_Personaje_Seleccionado();
            cuenta.Game.Character.timer_afk.Change(1200000, 1200000);
            cliente.account.accountState = AccountState.CONNECTED_INACTIVE;
        }
    }
}
