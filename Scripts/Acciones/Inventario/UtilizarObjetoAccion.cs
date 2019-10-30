using System.Threading.Tasks;
using Bot_Dofus_1._29._1.Game.Character.Inventory;
using Bot_Dofus_1._29._1.Managers;
using Bot_Dofus_1._29._1.Managers.Accounts;

namespace Bot_Dofus_1._29._1.Scripts.Acciones.Inventario
{
    public class UtilizarObjetoAccion : AccionesScript
    {
        public int modelo_id { get; private set; }

        public UtilizarObjetoAccion(int _modelo_id) => modelo_id = _modelo_id;

        internal override async Task<ResultadosAcciones> proceso(Account cuenta)
        {
            InventoryObject objeto = cuenta.game.CharacterClass.inventario.get_Objeto_Modelo_Id(modelo_id);

            if (objeto != null)
            {
                cuenta.game.CharacterClass.inventario.utilizar_Objeto(objeto);
                await Task.Delay(800);
            }

            return ResultadosAcciones.HECHO;
        }
    }
}
