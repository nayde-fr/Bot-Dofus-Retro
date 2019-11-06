using System;
using Bot_Dofus_1._29._1.Game.Mapas;
using Bot_Dofus_1._29._1.Managers.Accounts;
using Bot_Dofus_1._29._1.Managers.Jobs;
using Bot_Dofus_1._29._1.Managers.Movements;

namespace Bot_Dofus_1._29._1.Managers.Characters
{
    public class CharacterManager : IManager, IDisposable
    {
        private bool isDisposed;

        public MovementManager MovementManager { get; private set; }
        public GatheringManager GatheringManager { get; private set; }

        public CharacterManager(Account account, Map map, Character characterClass)
        {
            MovementManager = new MovementManager(account, map, characterClass);
            GatheringManager = new GatheringManager(account, MovementManager, map);
        }

        public void Clear()
        {
            MovementManager.Clear();
            GatheringManager.Clear();
        }

        #region Zona Dispose
        ~CharacterManager() => Dispose(false);
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            if (disposing)
            {
                MovementManager.Dispose();
            }

            MovementManager = null;
            isDisposed = true;
        }
        #endregion
    }
}
