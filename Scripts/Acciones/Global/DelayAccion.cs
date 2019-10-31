using System.Threading.Tasks;
using Bot_Dofus_1._29._1.Managers;
using Bot_Dofus_1._29._1.Managers.Accounts;

namespace Bot_Dofus_1._29._1.Scripts.Acciones.Global
{
    public class DelayAccion : AccionesScript
    {
        public int milisegundos { get; private set; }

        public DelayAccion(int ms) => milisegundos = ms;

        internal override async Task<ResultadosAcciones> proceso(Account cuenta)
        {
            await Task.Delay(milisegundos);
            return ResultadosAcciones.HECHO;
        }
    }
}
