using Bot_Dofus_1._29._1.Common.Frames.Transport;
using Bot_Dofus_1._29._1.Common.Network;

namespace Bot_Dofus_1._29._1.Common.Frames.Game
{
    internal class ChatFrame : Frame
    {
        [PacketHandler("cC+")]
        public void get_Agregar_Canal(TcpClient cliente, string paquete) => cliente.account.Game.Character.agregar_Canal_Personaje(paquete.Substring(3));

        [PacketHandler("cC-")]
        public void get_Eliminar_Canal(TcpClient cliente, string paquete) => cliente.account.Game.Character.eliminar_Canal_Personaje(paquete.Substring(3));

        [PacketHandler("cMK")]
        public void get_Mensajes_Chat(TcpClient cliente, string paquete)
        {
            string[] separador = paquete.Substring(3).Split('|');
            string canal = string.Empty;

            switch (separador[0])
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
                    cliente.account.logger.log_privado("Message Reçu", separador[2] + ": " + separador[3]);
                break;

                case "T":
                    cliente.account.logger.log_privado("Message Envoyé", separador[2] + ": " + separador[3]);
                break;

                default:
                    canal = "GENERAL";
                break;
            }

            if (!canal.Equals(string.Empty))
                cliente.account.logger.log_normal(canal, separador[2] + ": " + separador[3]);
        }
    }
}
