using System;
using Bot_Dofus_1._29._1.Game.Mapas.Interactivo;
using Bot_Dofus_1._29._1.Managers.Accounts;
using Bot_Dofus_1._29._1.Managers.Movements;

namespace Bot_Dofus_1._29._1.Managers.Interactives
{
    public class Interactive : IDisposable
    {
        private Account cuenta;
        private InteractiveObject interactivo_utilizado;

        public event Action<bool> fin_interactivo;
        private bool disposed;

        public Interactive(Account _cuenta, MovementManager movementManager)
        {
            cuenta = _cuenta;
            //movimiento.movimiento_finalizado += evento_Movimiento_Finalizado;
        }

        #region Zona Dispose
        public void Dispose() => Dispose(true);
        ~Interactive() => Dispose(false);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                interactivo_utilizado = null;
                cuenta = null;

                disposed = true;
            }
        }
        #endregion
    }
}
