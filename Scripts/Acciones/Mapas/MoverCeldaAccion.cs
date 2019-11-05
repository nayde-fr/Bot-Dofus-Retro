using System.Threading.Tasks;
using Bot_Dofus_1._29._1.Game.Mapas;
using Bot_Dofus_1._29._1.Managers.Accounts;
using Bot_Dofus_1._29._1.Managers.Movements;

namespace Bot_Dofus_1._29._1.Scripts.Acciones.Mapas
{
    public class MoverCeldaAccion : AccionesScript
    {
        public short celda_id { get; private set; }
        public MoverCeldaAccion(short _celda_id) => celda_id = _celda_id;

        internal override Task<ResultadosAcciones> proceso(Account cuenta)
        {
            Map mapa = cuenta.Game.Map;
            Cell celda = mapa.GetCellFromId(celda_id);

            if (celda == null)
                return resultado_fallado;

            switch (cuenta.Game.manager.MovementManager.get_Mover_A_Celda(celda, cuenta.Game.Map.celdas_ocupadas()))
            {
                case MovementResult.OK:
                    return resultado_procesado;

                case MovementResult.SAME_CELL:
                    return resultado_hecho;

                default:
                    return resultado_fallado;
            }

        }
    }
}
