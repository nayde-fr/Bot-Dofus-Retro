using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Bot_Dofus_1._29._1.Utilities.Config;
using Microsoft.VisualBasic;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
	Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.UserInterface.Forms
{
    public partial class GestionCuentas : Form
    {
        private List<AccountConfig> cuentas_cargadas;

        public GestionCuentas()
        {
            InitializeComponent();
            cuentas_cargadas = new List<AccountConfig>();
            foreach (var server in ConfigurationManager.Configuration.ServerInfos)
            {
                comboBox_Servidor.Items.Add(server);
            }
            comboBox_Servidor.SelectedIndex = 0;
            cargar_Cuentas_Lista();
        }

        private void cargar_Cuentas_Lista()
        {
            listViewCuentas.Items.Clear();

            ConfigurationManager.Configuration.Accounts.ForEach(x =>
            {
                if (!Principal.cuentas_cargadas.ContainsKey(x.Username))
                    listViewCuentas.Items.Add(x.Username).SubItems.AddRange(new string[2] { x.GetChosenServer().Name, x.CharacterName });
            });
        }

        private void boton_Agregar_Cuenta_Click(object sender, EventArgs e)
        {
            if (ConfigurationManager.GetAccount(textBox_Nombre_Cuenta.Text) != null && ConfigurationManager.Configuration.DebugPackets)
            {
                MessageBox.Show("Un compte existe déjà avec le nom du compte", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool tiene_errores = false;
            tableLayoutPanel6.Controls.OfType<TableLayoutPanel>().ToList().ForEach(panel =>
            {
                panel.Controls.OfType<TextBox>().ToList().ForEach(textbox =>
                {
                    if (string.IsNullOrEmpty(textbox.Text) || textbox.Text.Split(new char[0]).Length > 1)
                    {
                        textbox.BackColor = Color.Red;
                        tiene_errores = true;
                    }
                    else
                        textbox.BackColor = Color.White;
                });
            });

            if (!tiene_errores)
            {
                var serverId = (comboBox_Servidor.SelectedItem as ServerInfo)?.Id ?? 0;
                var realmId = (realCbx.SelectedItem as RealmInfo)?.Id ?? 0;
                ConfigurationManager.AddAccount(new AccountConfig(textBox_Nombre_Cuenta.Text, textBox_Password.Text, serverId, realmId, textBox_nombre_personaje.Text));
                cargar_Cuentas_Lista();

                textBox_Nombre_Cuenta.Clear();
                textBox_Password.Clear();
                textBox_nombre_personaje.Clear();

                if (checkBox_Agregar_Retroceder.Checked)
                    tabControlPrincipalCuentas.SelectedIndex = 0;

                ConfigurationManager.Save();
            }
        }

        private void listViewCuentas_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = listViewCuentas.Columns[e.ColumnIndex].Width;
        }

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCuentas.SelectedItems.Count > 0 && listViewCuentas.FocusedItem != null)
            {
                foreach (ListViewItem cuenta in listViewCuentas.SelectedItems)
                {
                    ConfigurationManager.DeleteAccount(cuenta.Index);
                    cuenta.Remove();
                }
                ConfigurationManager.Save();
                cargar_Cuentas_Lista();
            }
        }

        private void conectarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCuentas.SelectedItems.Count > 0 && listViewCuentas.FocusedItem != null)
            {
                foreach (ListViewItem cuenta in listViewCuentas.SelectedItems)
                    cuentas_cargadas.Add(ConfigurationManager.GetAccount( cuenta.Text));

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        public List<AccountConfig> get_Cuentas_Cargadas() => cuentas_cargadas;
        private void listViewCuentas_MouseDoubleClick(object sender, MouseEventArgs e) => conectarToolStripMenuItem.PerformClick();

        private void modificar_Cuenta(object sender, EventArgs e)
        {
            if (listViewCuentas.SelectedItems.Count == 1 && listViewCuentas.FocusedItem != null)
            {
                AccountConfig cuenta = ConfigurationManager.GetAccount(listViewCuentas.SelectedItems[0].Index);

                switch (sender.ToString())
                {
                    case "Cuenta":
                        string nueva_cuenta = Interaction.InputBox($"Entrez le nouveau compte", "Modifier le compte", cuenta.Username);

                        if (!string.IsNullOrEmpty(nueva_cuenta) || nueva_cuenta.Split(new char[0]).Length == 0)
                            cuenta.Username = nueva_cuenta;
                    break;

                    case "Contraseña":
                        string nueva_password = Interaction.InputBox($"Entrez le nouveau mot de passe", "Changer le mot de passe", cuenta.Password);

                        if (!string.IsNullOrEmpty(nueva_password) || nueva_password.Split(new char[0]).Length == 0)
                            cuenta.Password = nueva_password;
                    break;

                    default:
                        string nuevo_personaje = Interaction.InputBox($"Entrez le nouveau nom du personnage", "Modifier le nom du personnage", cuenta.CharacterName);
                        cuenta.CharacterName = nuevo_personaje;
                    break;
                }

                ConfigurationManager.Save();
                cargar_Cuentas_Lista();
            }
        }

        private void ComboBox_Servidor_SelectedIndexChanged(object sender, EventArgs e)
        {
            realCbx.Items.Clear();
            var serverInfo = (comboBox_Servidor.SelectedItem as ServerInfo);
            if (serverInfo == null)
            {
                return;
            }

            foreach (var realm in serverInfo.RealmInfos)
            {
                realCbx.Items.Add(realm);
            }
        }
    }
}
