using System;
using System.Collections.Generic;
using Bot_Dofus_1._29._1.Managers.Accounts;

namespace Bot_Dofus_1._29._1.Managers.Groups
{
    public class Group : IDisposable
    {
        private GroupManager grupo;
        private List<Account> miembros_perdidos;
        private bool disposed;

        public Group(GroupManager _grupo) => grupo = _grupo;

        #region Zona Dispose
        public void Dispose() => Dispose(true);
        ~Group() => Dispose(false);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                grupo = null;
                miembros_perdidos?.Clear();
                miembros_perdidos = null;

                disposed = true;
            }
        }
        #endregion
    }
}
