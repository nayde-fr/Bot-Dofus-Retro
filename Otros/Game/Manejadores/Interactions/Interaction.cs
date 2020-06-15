using Bot_Dofus_1._29._1.Otros.Game.Character;
using Bot_Dofus_1._29._1.Otros.Game.Entidades.Manejadores.Movimientos;
using Bot_Dofus_1._29._1.Otros.Mapas;
using Bot_Dofus_1._29._1.Otros.Mapas.Interactivo;
using Bot_Dofus_1._29._1.Otros.Mapas.Movimiento.Mapas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Bot_Dofus_1._29._1.Otros.Game.Character.Inventory;

namespace Bot_Dofus_1._29._1.Otros.Game.Entidades.Manejadores.Interactions
{
    public class Interaction : IDisposable
    {
        private Account account;
        private Map map;
        private Pathfinder pathfinder;

        //public event Action<bool> fin_interactivo;
        private bool disposed;

        public Interaction(Account account, Map map)
        {
            this.account = account;
            this.map = map;
            this.pathfinder = new Pathfinder();
        }

        private ObjetoInteractivo GetInteractiveObject(short cellId)
        {
            foreach (ObjetoInteractivo interactiveObject in map.interactives.Values)
            {
                if (cellId == interactiveObject.celda.cellId)
                    return interactiveObject;
            }

            return null;
        }

        private bool MoveToInteractiveObject(ObjetoInteractivo interactiveObject)
        {
            switch (account.game.manager.movimientos.get_Mover_A_Celda(interactiveObject.celda, map.celdas_ocupadas(), true, 1))
            {
                case ResultadoMovimientos.EXITO:
                case ResultadoMovimientos.SameCell:
                    return true;

                default:
                    return false;
            }
        }

        private void UseInteractiveObject(ObjetoInteractivo interactiveObject, short skillId)
        {
            if (account.game.character.get_SkillsId_Disponibles().Contains(skillId))
                account.connexion.SendPacket("GA500" + interactiveObject.celda.cellId + ";" + skillId);
        }

        private void MovementItem(bool add, uint itemId, int itemQuantity, int itemPrice = 0)
        {
            account.connexion.SendPacket("EMO" + (!add ? "-" : "+") + itemId + "|" + itemQuantity + (itemPrice != 0 ? "|" + itemPrice : ""), true);
        }

        private void Ready()
        {
            account.connexion.SendPacket("EK", true);
        }

        private void Leave()
        {
            account.connexion.SendPacket("EV", true);
        }

        private void RepeatCraft(int howManyTimes)
        {
            account.connexion.SendPacket("EMR" + howManyTimes);
        }

        public bool Craft(short interactiveObjectCellId, short skillId, List<InventoryObject> recipe, int quantity)
        {
            ObjetoInteractivo workbench = GetInteractiveObject(interactiveObjectCellId);

            if (workbench == null)
                return false;

            if (!MoveToInteractiveObject(workbench))
                return false;

            UseInteractiveObject(workbench, skillId);

            foreach(InventoryObject obj in recipe)
            {
                InventoryObject inventoryObject = account.game.character.inventario.get_Objeto_Modelo_Id(obj.id_modelo);

                if (inventoryObject == null)
                {
                    account.Logger.LogDanger("CRAFT", $"Impossible de trouver l'objet {obj.id_modelo} dans l'inventaire.");
                    return false;
                }

                MovementItem(true, inventoryObject.id_inventario, obj.cantidad);
            }

            Ready();

            RepeatCraft(quantity);

            return true;
        }

        #region Zona Dispose
        public void Dispose() => Dispose(true);
        ~Interaction() => Dispose(false);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                account = null;
                map = null;
                pathfinder = null;
                disposed = true;
            }
        }
        #endregion
    }
}
