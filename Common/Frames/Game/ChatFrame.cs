using Bot_Dofus_1._29._1.Common.Frames.Transport;
using Bot_Dofus_1._29._1.Common.Network;

namespace Bot_Dofus_1._29._1.Common.Frames.Game
{
    internal class ChatFrame : Frame
    {
        [PacketHandler("cC+")]
        public void AddChannelPacketHandle(TcpClient prmClient, string prmRawPacketData) => prmClient.account.Game.Character.agregar_Canal_Personaje(prmRawPacketData.Substring(3));

        [PacketHandler("cC-")]
        public void RemoveChannelPacketHandle(TcpClient prmClient, string prmRawPacketData) => prmClient.account.Game.Character.eliminar_Canal_Personaje(prmRawPacketData.Substring(3));

        [PacketHandler("cMK")]
        public void ChatMessagePacketHandle(TcpClient prmClient, string prmRawPacketData)
        {
            string[] splittedData = prmRawPacketData.Substring(3).Split('|');
            string canal = string.Empty;

            switch (splittedData[0])
            {
                case "?":
                    canal = "RECRUTEMENT";
                break;

                case ":":
                    canal = "COMMERCE";
                break;

                case "^":
                    canal = "INCARNAM";
                break;

                case "i":
                    canal = "INFORMATION";
                break;

                case "#":
                    canal = "EQUIPE";
                break;

                case "$":
                    canal = "GROUPE";
                break;

                case "%":
                    canal = "GUILDE";
                break;

                case "F":
                    prmClient.account.logger.log_privado("Message Reçu", splittedData[2] + ": " + splittedData[3]);
                break;

                case "T":
                    prmClient.account.logger.log_privado("Message Envoyé", splittedData[2] + ": " + splittedData[3]);
                break;

                default:
                    canal = "GENERAL";
                break;
            }

            if (!canal.Equals(string.Empty))
                prmClient.account.logger.log_normal(canal, splittedData[2] + ": " + splittedData[3]);
        }
    }
}
