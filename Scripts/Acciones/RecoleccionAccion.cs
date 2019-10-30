using System.Collections.Generic;
using System.Threading.Tasks;
using Bot_Dofus_1._29._1.Managers;
using Bot_Dofus_1._29._1.Managers.Accounts;

namespace Bot_Dofus_1._29._1.Scripts.Acciones
{
    internal class RecoleccionAccion : AccionesScript
    {
        public List<short> elementos { get; private set; }

        public RecoleccionAccion(List<short> _elementos) => elementos = _elementos;

        internal override Task<ResultadosAcciones> proceso(Account cuenta)
        {
            if (cuenta.game.manager.GatheringManager.get_Puede_Recolectar(elementos))
            {
                if (!cuenta.game.manager.GatheringManager.get_Recolectar(elementos))
                    return resultado_fallado;

                return resultado_procesado;
            }
            return resultado_hecho;
        }
    }
}
