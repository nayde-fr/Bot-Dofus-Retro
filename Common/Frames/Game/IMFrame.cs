using Bot_Dofus_1._29._1.Common.Frames.Transport;
using Bot_Dofus_1._29._1.Common.Network;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Common.Frames.Game
{
    class IMFrame : Frame
    {
        [PacketHandler("Im189")]
        public void GetWelcomeMessage(TcpClient prmClient, string prmRawPacketDatas) => prmClient.account.logger.log_Error("DOFUS", "Bienvenue à DOFUS, le Monde des Douze ! Attention Il est interdit de communiquer le nom d'utilisateur et le mot de passe de votre compte.");

        [PacketHandler("Im039")]
        public void GetSpectatorModeDisabledMessage(TcpClient prmClient, string prmRawPacketDatas) => prmClient.account.logger.log_informacion("COMBAT", "Le mode Spectator est désactivé.");

        [PacketHandler("Im040")]
        public void GetSpectatorModeEnsabledMessage(TcpClient prmClient, string prmRawPacketDatas) => prmClient.account.logger.log_informacion("COMBAT", "Le mode Spectator est activé.");

        [PacketHandler("Im0152")]
        public void GetLastConnexionIpMessage(TcpClient prmClient, string prmRawPacketDatas)
        {
            string msg = prmRawPacketDatas.Substring(3).Split(';')[1];
            prmClient.account.logger.log_informacion("DOFUS", "Dernière connexion à votre compte effectuée le " + msg.Split('~')[0] + "/" + msg.Split('~')[1] + "/" + msg.Split('~')[2] + " à " + msg.Split('~')[3] + ":" + msg.Split('~')[4] + " adresse IP " + msg.Split('~')[5]);
        }

        [PacketHandler("Im0153")]
        public void GetNewConnexionIpMessage(TcpClient prmClient, string prmRawPacketDatas) => prmClient.account.logger.log_informacion("DOFUS", "Votre adresse IP actuelle est " + prmRawPacketDatas.Substring(3).Split(';')[1]);

        [PacketHandler("Im020")]
        public void GetChestOpeningPrinceMessage(TcpClient prmClient, string prmRawPacketDatas) => prmClient.account.logger.log_informacion("DOFUS", "Vous avez dû donner " + prmRawPacketDatas.Split(';')[1] + " kamas pour accéder à ce coffre.");

        [PacketHandler("Im025")]
        public void GetHappyPetMessage(TcpClient prmClient, string prmRawPacketDatas) => prmClient.account.logger.log_informacion("DOFUS", "Votre animal est si heureux de vous revoir !");

        [PacketHandler("Im0157")]
        public void GetChatDiffusionErrorMessage(TcpClient prmClient, string prmRawPacketDatas) => prmClient.account.logger.log_informacion("DOFUS", "Ce canal est seulement disponible aux abonnés de niveau " + prmRawPacketDatas.Split(';')[1]);

        [PacketHandler("Im037")]
        public void GetAwayModeEnabledMessage(TcpClient prmClient, string prmRawPacketDatas) => prmClient.account.logger.log_informacion("DOFUS", "Désormais, tu seras considéré comme absent.");

        [PacketHandler("Im112")]
        public void GetFullPodsMessage(TcpClient prmClient, string prmRawPacketDatas) => prmClient.account.logger.log_Error("DOFUS", "Tu es trop chargé. Jetez quelques objets pour pouvoir bouger.");
    }
}
