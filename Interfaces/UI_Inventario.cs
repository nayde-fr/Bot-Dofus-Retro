﻿using Bot_Dofus_1._29._1.Otros;
using Bot_Dofus_1._29._1.Otros.Game.Character.Inventory;
using Bot_Dofus_1._29._1.Otros.Game.Character.Inventory.Enums;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bot_Dofus_1._29._1.Interfaces
{
    public partial class UI_Inventario : UserControl
    {
        private Account cuenta = null;

        public UI_Inventario(Account _cuenta)
        {
            InitializeComponent();
            set_DoubleBuffered(dataGridView_equipamientos);
            set_DoubleBuffered(dataGridView_varios);
            set_DoubleBuffered(dataGridView_recursos);

            cuenta = _cuenta;
            cuenta.game.character.inventario.inventario_actualizado += actualizar_Inventario;
        }

        private void actualizar_Inventario(bool objetos_actualizados)
        {
            if (!objetos_actualizados)
                return;

            Task.Run(() =>
            {
                if (!IsHandleCreated)
                    return;

                BeginInvoke((Action)(() =>
                {
                    dataGridView_equipamientos.Rows.Clear();
                    foreach (InventoryObject obj in cuenta.game.character.inventario.equipamiento)
                        dataGridView_equipamientos.Rows.Add(new object[] { obj.id_inventario, obj.id_modelo, obj.nombre, obj.cantidad, obj.posicion, obj.posicion == InventorySlots.NOT_EQUIPPED ? "Equipar" : "Déséquiper", "Détruire" });

                    dataGridView_varios.Rows.Clear();
                    foreach (InventoryObject obj in cuenta.game.character.inventario.varios)
                        dataGridView_varios.Rows.Add(new object[] { obj.id_inventario, obj.id_modelo, obj.nombre, obj.cantidad, obj.pods, "Détruire" });

                    dataGridView_recursos.Rows.Clear();
                    foreach (InventoryObject obj in cuenta.game.character.inventario.recursos)
                        dataGridView_recursos.Rows.Add(new object[6] { obj.id_inventario, obj.id_modelo, obj.nombre, obj.cantidad, obj.pods, "Détruire" });

                    dataGridView_mision.Rows.Clear();
                    foreach (InventoryObject obj in cuenta.game.character.inventario.mision)
                        dataGridView_mision.Rows.Add(new object[4] { obj.id_inventario, obj.id_modelo, obj.nombre, obj.cantidad });

                }));
            });
        }

        private void dataGridView_equipamientos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 5)
                return;

            InventoryObject objeto = cuenta.game.character.inventario.equipamiento.ElementAt(e.RowIndex);
            
            string accion = dataGridView_equipamientos.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            switch (accion)
            {
                case "Equipar":
                    cuenta.game.character.inventario.equipar_Objeto(objeto);
                break;

                case "Desequipar":
                    cuenta.game.character.inventario.desequipar_Objeto(objeto);
                break;

                case "Eliminar":
                    if (MessageBox.Show("Realmente deseas eliminar " + objeto.nombre + "?", "Eliminar un objeto", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        cuenta.game.character.inventario.eliminar_Objeto(objeto, 1, true);
                break;
            }
        }

        private void dataGridView_recursos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 5)
                return;

            InventoryObject objeto = cuenta.game.character.inventario.recursos.ElementAt(e.RowIndex);
            string accion = dataGridView_recursos.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            if (!int.TryParse(Interaction.InputBox($"Ingresa la cantidad para {accion.ToLower()} el objeto {objeto.nombre} (0 = todos):", accion, "1"), out int cantidad))
                return;

            switch (accion)
            {
                case "Eliminar":
                    cuenta.game.character.inventario.eliminar_Objeto(objeto, cantidad, true);
                break;
            }
        }

        public static void set_DoubleBuffered(Control control) => typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, control, new object[] { true });
    }
}
