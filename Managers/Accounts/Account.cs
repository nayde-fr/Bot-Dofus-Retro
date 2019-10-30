using System;
using System.Net;
using Bot_Dofus_1._29._1.Common.Network;
using Bot_Dofus_1._29._1.Game.Enums;
using Bot_Dofus_1._29._1.Managers.Fights;
using Bot_Dofus_1._29._1.Managers.Groups;
using Bot_Dofus_1._29._1.Scripts;
using Bot_Dofus_1._29._1.Utilities.Config;
using Bot_Dofus_1._29._1.Utilities.Logs;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
	Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Managers.Accounts
{
    public class Account : IDisposable
    {
        public string nickname { get; set; } = string.Empty;
        public string welcomeKey { get; set; } = string.Empty;
        public string gameTicket { get; set; } = string.Empty;
        public Logger logger { get; private set; }
        public TcpClient connexion { get; set; }
        public AccountManager game { get; private set; }
        public ManejadorScript script { get; set; }
        public PeleaExtensiones fightExtension { get; set; }
        public AccountConfig accountConfig { get; private set; }
        private AccountState _accountState = AccountState.DISCONNECTED;
        public bool canUseMount = false;

        public GroupManager group { get; set; }
        public bool hasGroup => group != null;
        public bool isGroupLeader => !hasGroup || group.lider == this;

        private bool _disposed;
        public event Action accountStateEvent;
        public event Action accountDisconnectEvent;

        public Account(AccountConfig prmAccountConfig)
        {
            accountConfig = prmAccountConfig;
            logger = new Logger();
            game = new AccountManager(this);
            fightExtension = new PeleaExtensiones(this);
            script = new ManejadorScript(this);
        }

        public void Connect()
        {
            connexion = new TcpClient(this);
            connexion.ConnectToServer(IPAddress.Parse(GlobalConfig.loginIP), GlobalConfig.loginPort);
        }

        public void Disconnect()
        {
            connexion?.Dispose();
            connexion = null;

            script.detener_Script();
            game.Clear();
            accountState = AccountState.DISCONNECTED;
            accountDisconnectEvent?.Invoke();
        }

        public void SwitchToGameServer(string ip, int port)
        {
            connexion.DisconnectSocket();
            connexion.ConnectToServer(IPAddress.Parse(ip), port);
        }

        public AccountState accountState
        {
            get => _accountState;
            set
            {
                _accountState = value;
                accountStateEvent?.Invoke();
            }
        }

        public bool Is_Busy() => accountState != AccountState.CONNECTED_INACTIVE && accountState != AccountState.REGENERATION;
        public bool Is_In_Dialog() => accountState == AccountState.STORAGE || accountState == AccountState.DIALOG || accountState == AccountState.EXCHANGE || accountState == AccountState.BUYING || accountState == AccountState.SELLING;
        public bool IsFighting() => accountState == AccountState.FIGHTING;
        public bool IsGathering() => accountState == AccountState.GATHERING;
        public bool IsMoving() => accountState == AccountState.MOVING;

        #region Zona Dispose
        public void Dispose() => Dispose(true);
        ~Account() => Dispose(false);

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    script.Dispose();
                    connexion?.Dispose();
                    game.Dispose();
                }
                accountState = AccountState.DISCONNECTED;
                script = null;
                welcomeKey = null;
                connexion = null;
                logger = null;
                game = null;
                nickname = null;
                accountConfig = null;
                _disposed = true;
            }
        }
        #endregion
    }
}
