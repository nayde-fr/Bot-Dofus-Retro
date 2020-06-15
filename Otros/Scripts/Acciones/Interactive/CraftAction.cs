using Bot_Dofus_1._29._1.Otros.Game.Character.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot_Dofus_1._29._1.Otros.Scripts.Acciones.Interactive
{
    public class CraftAction : ScriptAction
    {
        public short cellId;
        public short skillId;
        public List<InventoryObject> recipe;
        public int quantity;

        public CraftAction(short cellId, short skillId, List<InventoryObject> recipe, int quantity)
        {
            this.cellId = cellId;
            this.skillId = skillId;
            this.recipe = recipe;
            this.quantity = quantity;
        }

        internal override Task<ResultadosAcciones> process(Account account)
        {
            if (!account.game.manager.interaction.Craft(cellId, skillId, recipe, quantity))
            {
                account.Logger.LogError("SCRIPT", $"Impossible d'ouvrir l'atelier en {cellId}.");
                return resultado_fallado;
            }

            account.Logger.LogInfo("SCRIPT", "Ouverture de l'atelier.");
            return resultado_procesado;
        }
    }
}
