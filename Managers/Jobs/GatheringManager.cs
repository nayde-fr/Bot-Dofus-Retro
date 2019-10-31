using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot_Dofus_1._29._1.Game.Character;
using Bot_Dofus_1._29._1.Game.Character.Inventory;
using Bot_Dofus_1._29._1.Game.Character.Inventory.Enums;
using Bot_Dofus_1._29._1.Game.Enums;
using Bot_Dofus_1._29._1.Game.Mapas;
using Bot_Dofus_1._29._1.Game.Mapas.Interactivo;
using Bot_Dofus_1._29._1.Game.Mapas.Movimiento.Mapas;
using Bot_Dofus_1._29._1.Managers.Accounts;
using Bot_Dofus_1._29._1.Managers.Characters;
using Bot_Dofus_1._29._1.Managers.Movements;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Managers.Jobs
{
    public class GatheringManager : IDisposable
    {
        private Account account;
        private Map map;
        public InteractiveObject gatheredInteractive;
        private List<int> ungatherableInteractives;
        private bool isStolen;
        private Pathfinder pathfinder;
        private bool disposed;

        public event Action onGatherStart;
        public event Action<GatheringResult> onGatherEnd;

        public static readonly int[] fishingTools = { 8541, 6661, 596, 1866, 1865, 1864, 1867, 2188, 1863, 1862, 1868, 1861, 1860, 2366 };
        
        public GatheringManager(Account account, MovementManager movementsManager, Map map)
        {
            this.account = account;
            ungatherableInteractives = new List<int>();
            pathfinder = new Pathfinder();
            this.map = map;

            movementsManager.movimiento_finalizado += get_Movimiento_Finalizado;
            map.mapRefreshEvent += EventoMapActualizado;
        }

        public bool get_Puede_Recolectar(List<short> elementos_recolectables) => get_Interactivos_Utilizables(elementos_recolectables).Count > 0;
        public void get_Cancelar_Interactivo() => gatheredInteractive = null;

        public bool get_Recolectar(List<short> elementos)
        {
            if (account.Is_Busy() || gatheredInteractive != null)
                return false;

            foreach (KeyValuePair<short, InteractiveObject> kvp in get_Interactivos_Utilizables(elementos))
            {
                if (get_Intentar_Mover_Interactivo(kvp))
                    return true;
            }

            account.logger.log_Peligro("RECOLTE", "Aucun objet de collectable trouvé");
            return false;
        }

        private Dictionary<short, InteractiveObject> get_Interactivos_Utilizables(List<short> elementos_ids)
        {
            Dictionary<short, InteractiveObject> elementos_utilizables = new Dictionary<short, InteractiveObject>();
            Character personaje = account.Game.Character;

            InventoryObject arma = personaje.inventario.get_Objeto_en_Posicion(InventorySlots.WEAPON);
            byte distancia_arma = 1;
            bool es_herramienta_pescar = false;

            if (arma != null)
            {
                distancia_arma = get_Distancia_herramienta(arma.id_modelo);
                es_herramienta_pescar = fishingTools.Contains(arma.id_modelo);
            }

            foreach (InteractiveObject interactivo in map.interactives.Values)
            {
                if (!interactivo.IsUsable || !interactivo.Model.recolectable)
                    continue;

                List<Cell> path = pathfinder.get_Path(personaje.Cell, interactivo.Cell, map.celdas_ocupadas(), true, distancia_arma);

                if (path == null || path.Count == 0)
                    continue;

                foreach (short habilidad in interactivo.Model.habilidades)
                {
                    if (!elementos_ids.Contains(habilidad))
                        continue;

                    if (!es_herramienta_pescar && path.Last().GetDistanceBetweenCells(interactivo.Cell) > 1)
                        continue;

                    if (es_herramienta_pescar && path.Last().GetDistanceBetweenCells(interactivo.Cell) > distancia_arma)
                        continue;

                    elementos_utilizables.Add(interactivo.Cell.cellId, interactivo);
                }
            }
            
            return elementos_utilizables;
        }

        private bool get_Intentar_Mover_Interactivo(KeyValuePair<short, InteractiveObject> interactivo)
        {
            gatheredInteractive = interactivo.Value;
            byte distancia_detener = 1;
            InventoryObject arma = account.Game.Character.inventario.get_Objeto_en_Posicion(InventorySlots.WEAPON);

            if(arma != null)
                distancia_detener = get_Distancia_herramienta(arma.id_modelo);
            
            switch (account.Game.manager.MovementManager.get_Mover_A_Celda(gatheredInteractive.Cell, map.celdas_ocupadas(), true, distancia_detener))
            {
                case MovementResult.OK:
                case MovementResult.SAME_CELL:
                    get_Intentar_Recolectar_Interactivo();
                return true;

                default:
                    get_Cancelar_Interactivo();
               return false;
            }
        }

        private void get_Intentar_Recolectar_Interactivo()
        {
            if (!isStolen)
            {
                foreach (short habilidad in gatheredInteractive.Model.habilidades)
                {
                    if (account.Game.Character.get_Skills_Recoleccion_Disponibles().Contains(habilidad))
                        account.connexion.SendPacket("GA500" + gatheredInteractive.Cell.cellId + ";" + habilidad);
                }
            }
            else
                evento_Recoleccion_Acabada(GatheringResult.STOLEN, gatheredInteractive.Cell.cellId);
        }

        private void get_Movimiento_Finalizado(bool correcto)
        {
            if (gatheredInteractive == null)
                return;

            if (!correcto && account.Game.manager.MovementManager.actual_path != null)
                evento_Recoleccion_Acabada(GatheringResult.FAILED, gatheredInteractive.Cell.cellId);
        }

        public async Task evento_Recoleccion_Iniciada(int id_personaje, int tiempo_delay, short celda_id, byte tipo_gkk)
        {
            if (gatheredInteractive == null || gatheredInteractive.Cell.cellId != celda_id)
                return;

            if (account.Game.Character.Id != id_personaje)
            {
                isStolen = true;
                account.logger.log_informacion("INFORMATION", "Un personnage a volé votre ressource.");
                evento_Recoleccion_Acabada(GatheringResult.STOLEN, gatheredInteractive.Cell.cellId);
            }
            else
            {
                account.accountState = AccountState.GATHERING;
                onGatherStart?.Invoke();
                await Task.Delay(tiempo_delay);
                account.connexion.SendPacket("GKK" + tipo_gkk);
            }
        }

        public void evento_Recoleccion_Acabada(GatheringResult resultado, short celda_id)
        {
            if (gatheredInteractive == null || gatheredInteractive.Cell.cellId != celda_id)
                return;

            isStolen = false;
            gatheredInteractive = null;
            account.accountState = AccountState.CONNECTED_INACTIVE;
            onGatherEnd?.Invoke(resultado);
        }

        private void EventoMapActualizado()
        {
            pathfinder.set_Mapa(account.Game.Map);
            ungatherableInteractives.Clear();
        }

        public byte get_Distancia_herramienta(int id_objeto)
        {
            switch (id_objeto)
            {
                case 8541://Caña de aprendiz de pescador
                case 6661://Caña para kuakuás
                case 596://Caña de pescar corta
                    return 2;

                case 1866://Caña de pescar estándar
                    return 3;

                case 1865://Caña cúbica
                case 1864://La aguja de tejer
                    return 4;

                case 1867://Gran caña de pescar
                case 2188://Caña para pischis
                    return 5;

                case 1863://Caña Jillo
                case 1862://El Palo de Amor
                    return 6;

                case 1868:
                    return 7;

                case 1861://La Gran Perca
                case 1860://Caña arpón
                    return 8;

                case 2366://Caña para kralamares
                    return 9;
            }

            return 1;
        }

        public void Clear()
        {
            gatheredInteractive = null;
            ungatherableInteractives.Clear();
            isStolen = false;
        }

        #region Zona Dispose
        ~GatheringManager() => Dispose(false);
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    pathfinder.Dispose();
                }

                ungatherableInteractives.Clear();
                ungatherableInteractives = null;
                gatheredInteractive = null;
                pathfinder = null;
                account = null;
                disposed = true;
            }
        }
        #endregion
    }
}
