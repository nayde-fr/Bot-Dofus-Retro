using System.Collections.Generic;
using System.Linq;
using Bot_Dofus_1._29._1.Common.Frames.Transport;
using Bot_Dofus_1._29._1.Common.Network;
using Bot_Dofus_1._29._1.Game.Enums;
using Bot_Dofus_1._29._1.Game.Mapas.Entidades;
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
    class NPCFrame : Frame
    {
        [PacketHandler("DCK")]
        public void get_Dialogo_Creado(TcpClient cliente, string paquete)
        {
            Account cuenta = cliente.account;

            cuenta.accountState = AccountState.DIALOG;
            cuenta.Game.Character.hablando_npc_id = sbyte.Parse(paquete.Substring(3));
        }

        [PacketHandler("DQ")]
        public void get_Lista_Respuestas(TcpClient cliente, string paquete)
        {
            Account cuenta = cliente.account;

            if (!cuenta.Is_In_Dialog())
                return;

            IEnumerable<Npcs> npcs = cuenta.Game.Map.lista_npcs();
            Npcs npc = npcs.ElementAt((cuenta.Game.Character.hablando_npc_id * -1) - 1);

            if (npc != null)
            {
                string[] pregunta_separada = paquete.Substring(2).Split('|');
                string[] respuestas_disponibles = pregunta_separada[1].Split(';');

                npc.pregunta = short.Parse(pregunta_separada[0].Split(';')[0]);
                npc.respuestas = new List<short>(respuestas_disponibles.Count());

                foreach (string respuesta in respuestas_disponibles)
                    npc.respuestas.Add(short.Parse(respuesta));

                cuenta.Game.Character.evento_Dialogo_Recibido();
            }
        }
    }
}
