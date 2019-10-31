using System;
using Bot_Dofus_1._29._1.Game.Character.Inventory;
using Bot_Dofus_1._29._1.Game.Character.Inventory.Enums;
using Bot_Dofus_1._29._1.Managers;
using Bot_Dofus_1._29._1.Managers.Accounts;
using Bot_Dofus_1._29._1.Scripts.Acciones.Inventario;
using Bot_Dofus_1._29._1.Scripts.Manejadores;
using MoonSharp.Interpreter;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Scripts.Api
{
    [MoonSharpUserData]
    public class InventarioApi : IDisposable
    {
        private Account cuenta;
        private ManejadorAcciones manejar_acciones;
        private bool disposed = false;

        public InventarioApi(Account _cuenta, ManejadorAcciones _manejar_acciones)
        {
            cuenta = _cuenta;
            manejar_acciones = _manejar_acciones;
        }

        public int pods() => cuenta.Game.Character.inventario.pods_actuales;
        public int podsMaximos() => cuenta.Game.Character.inventario.pods_maximos;
        public int podsPorcentaje() => cuenta.Game.Character.inventario.porcentaje_pods;
        public bool tieneObjeto(int modelo_id) => cuenta.Game.Character.inventario.get_Objeto_Modelo_Id(modelo_id) != null;

        public bool utilizar(int modelo_id)
        {
            InventoryObject objeto = cuenta.Game.Character.inventario.get_Objeto_Modelo_Id(modelo_id);

            if (objeto == null)
                return false;

            manejar_acciones.enqueue_Accion(new UtilizarObjetoAccion(modelo_id), true);
            return true;
        }

        public bool equipar(int modelo_id)
        {
            InventoryObject objeto = cuenta.Game.Character.inventario.get_Objeto_Modelo_Id(modelo_id);

            if (objeto == null || objeto.posicion != InventorySlots.NOT_EQUIPPED)
                return false;

            manejar_acciones.enqueue_Accion(new EquiparItemAccion(modelo_id), true);
            return true;
        }

        #region Zona Dispose
        ~InventarioApi() => Dispose(false);
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                cuenta = null;
                manejar_acciones = null;
                disposed = true;
            }
        }
        #endregion
    }
}
