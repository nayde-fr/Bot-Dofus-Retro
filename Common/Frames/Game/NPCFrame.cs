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
        public void GetCreatedDialogue(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;

            account.accountState = AccountState.DIALOG;
            account.Game.Character.hablando_npc_id = sbyte.Parse(prmRawPacketData.Substring(3));
        }

        [PacketHandler("DQ")]
        public void GetAnswersList(TcpClient prmClient, string prmRawPacketData)
        {
            Account account = prmClient.account;

            if (!account.Is_In_Dialog())
                return;

            IEnumerable<Npcs> npcs = account.Game.Map.lista_npcs();
            Npcs npc = npcs.ElementAt((account.Game.Character.hablando_npc_id * -1) - 1);

            if (npc != null)
            {
                string[] questionsSplitter = prmRawPacketData.Substring(2).Split('|');
                string[] availableAnswers = questionsSplitter[1].Split(';');

                npc.pregunta = short.Parse(questionsSplitter[0].Split(';')[0]);
                npc.respuestas = new List<short>(availableAnswers.Count());

                foreach (string answer in availableAnswers)
                    npc.respuestas.Add(short.Parse(answer));

                account.Game.Character.evento_Dialogo_Recibido();
            }
        }
    }
}
