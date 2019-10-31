using System.Collections.Generic;
using System.Threading.Tasks;
using Bot_Dofus_1._29._1.Game.Mapas;
using Bot_Dofus_1._29._1.Game.Mapas.Entidades;
using Bot_Dofus_1._29._1.Managers;
using Bot_Dofus_1._29._1.Managers.Accounts;
using Bot_Dofus_1._29._1.Managers.Movements;

namespace Bot_Dofus_1._29._1.Scripts.Acciones
{
    public class PeleasAccion : AccionesScript
    {
        public int monstruos_minimos { get; private set; }
        public int monstruos_maximos { get; private set; }
        public int monstruo_nivel_minimo { get; private set; }
        public int monstruo_nivel_maximo { get; private set; }
        public List<int> monstruos_prohibidos { get; private set; }
        public List<int> monstruos_obligatorios { get; private set; }

        public PeleasAccion(int _monstruos_minimos, int _monstruos_maximos, int _monstruo_nivel_minimo, int _monstruo_nivel_maximo, List<int> _monstruos_prohibidos, List<int> _monstruos_obligatorios)
        {
            monstruos_minimos = _monstruos_minimos;
            monstruos_maximos = _monstruos_maximos;
            monstruo_nivel_minimo = _monstruo_nivel_minimo;
            monstruo_nivel_maximo = _monstruo_nivel_maximo;
            monstruos_prohibidos = _monstruos_prohibidos;
            monstruos_obligatorios = _monstruos_obligatorios;
        }

        internal override Task<ResultadosAcciones> proceso(Account cuenta)
        {
            Map mapa = cuenta.Game.Map;
            List<Monstruos> grupos_disponibles = mapa.get_Grupo_Monstruos(monstruos_minimos, monstruos_maximos, monstruo_nivel_minimo, monstruo_nivel_maximo, monstruos_prohibidos, monstruos_obligatorios);

            if (grupos_disponibles.Count > 0)
            {
                foreach (Monstruos grupo_monstruo in grupos_disponibles)
                {
                    var test = cuenta.Game.manager.MovementManager.get_Mover_A_Celda(grupo_monstruo.Cell, new List<Cell>());
                    switch (test)
                    {
                        case MovementResult.OK:
                            cuenta.logger.log_informacion("SCRIPT", $"Mouvent vers un groupes à la cellule : {grupo_monstruo.Cell.cellId}, monstres total: {grupo_monstruo.get_Total_Monstruos}, niveaux du groupe: {grupo_monstruo.get_Total_Nivel_Grupo}");
                        return resultado_procesado;
                            
                        case MovementResult.PATHFINDING_ERROR:
                        case MovementResult.SAME_CELL:
                            cuenta.logger.log_Peligro("SCRIPT", "Le chemin vers le groupe de monstres est bloqué.");
                        continue;

                        default:
                            cuenta.script.detener_Script("C'est trompé de groupes" + test);
                        return resultado_fallado;
                    }
                }
            }

            return resultado_hecho;
        }
    }
}
