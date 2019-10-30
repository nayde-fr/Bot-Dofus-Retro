using System;
using Bot_Dofus_1._29._1.Game.Enums;
using Bot_Dofus_1._29._1.Managers;

namespace Bot_Dofus_1._29._1.Game.Server
{
    public class GameServer : IManager, IDisposable
    {
        public int serverId;
        public string serverName;
        public ServerState serverState;
        private bool _disposed = false;

        public GameServer() => RefreshData(0, "UNDEFINED", ServerState.OFFLINE);

        public void RefreshData(int prmId, string prmServerName, ServerState prmServerState)
        {
            serverId = prmId;
            serverName = prmServerName;
            serverState = prmServerState;
        }

        public string GetState(ServerState state)
        {
            switch (state)
            {
                case ServerState.ONLINE:
                    return "En-Ligne";
                case ServerState.SAVING:
                    return "Sauvegarde";
                case ServerState.OFFLINE:
                    return "Hors-Ligne";
                default:
                    return "";
            }
        }

        #region Zona Dispose
        public void Dispose() => Dispose(true);
        ~GameServer() => Dispose(false);

        public void Clear()
        {
            serverId = 0;
            serverName = null;
            serverState = ServerState.OFFLINE;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            serverId = 0;
            serverName = null;
            serverState = ServerState.OFFLINE;
            _disposed = true;
        }
        #endregion
    }
}
