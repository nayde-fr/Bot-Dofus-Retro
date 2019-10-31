using System.Threading.Tasks;
using Bot_Dofus_1._29._1.Game.Character.Inventory;
using Bot_Dofus_1._29._1.Managers;
using Bot_Dofus_1._29._1.Managers.Accounts;

namespace Bot_Dofus_1._29._1.Scripts.Acciones.Inventario
{
    class EquiparItemAccion : AccionesScript
    {
        public int modelo_id { get; private set; }

        public EquiparItemAccion(int _modelo_id)
        {
            modelo_id = _modelo_id;
        }

        internal override async Task<ResultadosAcciones> proceso(Account cuenta)
        {
            InventoryObject objeto = cuenta.Game.Character.inventario.get_Objeto_Modelo_Id(modelo_id);

            if (objeto != null && cuenta.Game.Character.inventario.equipar_Objeto(objeto))
                await Task.Delay(500);

            return ResultadosAcciones.HECHO;
        }
    }
}
