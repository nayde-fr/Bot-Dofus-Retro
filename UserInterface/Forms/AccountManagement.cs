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
    public partial class AccountManagement : Form
    {
        private List<AccountConfig> LoadedAccounts;

        public AccountManagement()
        {
            InitializeComponent();
            LoadedAccounts = new List<AccountConfig>();

            foreach (var server in ConfigurationManager.Configuration.ServerInfos)
                comboBoxServer.Items.Add(server);

            comboBoxServer.SelectedIndex = 0;
            LoadAccountList();
        }

        private void LoadAccountList()
        {
            listViewAccounts.Items.Clear();

            ConfigurationManager.Configuration.Accounts.ForEach(account => 
            {
                if (!Principal.LoadedAccounts.ContainsKey(account.Username))
                    listViewAccounts.Items.Add(account.Username).SubItems.AddRange(new[] 
                    { 
                        account.GetChosenServer().Name, 
                        account.CharacterName
                    });
            });
        }

        private void ButtonAddAccountClick(object sender, EventArgs e)
        {
            bool hasErrors = false;
            tableLayoutPanel6.Controls.OfType<TableLayoutPanel>().ToList().ForEach(panel =>
            {
                panel.Controls.OfType<TextBox>().ToList().ForEach(textbox =>
                {
                    if (string.IsNullOrEmpty(textbox.Text) || textbox.Text.Split().Length > 1)
                    {
                        textbox.BackColor = Color.Red;
                        hasErrors = true;
                    }
                    else
                        textbox.BackColor = Color.White;
                });
            });

            if (hasErrors) return;

            var serverId = (comboBoxServer.SelectedItem as ServerInfo)?.Id ?? 0;
            var realmId = (realCbx.SelectedItem as RealmInfo)?.Id ?? 0;
            ConfigurationManager.AddAccount(new AccountConfig(textBoxAccountName.Text, textBoxPassword.Text, serverId, realmId, textBoxCharacterName.Text));
            LoadAccountList();

            textBoxAccountName.Clear();
            textBoxPassword.Clear();
            textBoxCharacterName.Clear();

            if (checkBox_Agregar_Retroceder.Checked) 
                tabControlPrincipalAccounts.SelectedIndex = 0;

            ConfigurationManager.Save();
        }

        private void ListViewAccountsColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = listViewAccounts.Columns[e.ColumnIndex].Width;
        }

        private void RemoveToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (listViewAccounts.SelectedItems.Count <= 0 || listViewAccounts.FocusedItem == null) return;

            foreach (ListViewItem account in listViewAccounts.SelectedItems)
            {
                ConfigurationManager.DeleteAccount(account.Index);
                account.Remove();
            }

            ConfigurationManager.Save();
            LoadAccountList();
        }

        private void ConnectToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (listViewAccounts.SelectedItems.Count <= 0 || listViewAccounts.FocusedItem == null) return;

            foreach (ListViewItem account in listViewAccounts.SelectedItems)
                LoadedAccounts.Add(ConfigurationManager.GetAccount(account.Text));

            DialogResult = DialogResult.OK;
            Close();
        }

        public List<AccountConfig> GetLoadedAccounts() => LoadedAccounts;
        private void ListViewAccountsDoubleClick(object sender, MouseEventArgs e) => conectarToolStripMenuItem.PerformClick();

        private void ModifyAccount(object sender, EventArgs e)
        {
            if (listViewAccounts.SelectedItems.Count != 1 || listViewAccounts.FocusedItem == null) return;
            var acccount = ConfigurationManager.GetAccount(listViewAccounts.SelectedItems[0].Index);

            switch (sender.ToString())
            {
                case "Compte": // Cuenta
                    string newAccountName = Interaction.InputBox("Entrez le nouveau compte", "Modifier le compte", acccount.Username);
                    if (!string.IsNullOrEmpty(newAccountName) || newAccountName.Split(new char[0]).Length == 0)
                    {
                        acccount.Username = newAccountName;
                        MessageBox.Show(string.Format("Changed account username to: {0}", newAccountName), "Success", MessageBoxButtons.OK);
                    }
                    break;
                case "Mot de passe": // Contraseña
                    string newAccountPassword = Interaction.InputBox("Entrez le nouveau mot de passe", "Changer le mot de passe", acccount.Password);
                    if (!string.IsNullOrEmpty(newAccountPassword) || newAccountPassword.Split(new char[0]).Length == 0)
                    {
                        acccount.Password = newAccountPassword;
                        MessageBox.Show(string.Format("Changed account password to: {0}", newAccountPassword), "Success", MessageBoxButtons.OK);
                    }
                    break;
                case "Nom du personnage": // Nombre de personna
                    string newCharacterName = Interaction.InputBox("Entrez le nouveau nom du personnage", "Modifier le nom du personnage", acccount.CharacterName);
                    if (!string.IsNullOrEmpty(newCharacterName) || newCharacterName.Split(new char[0]).Length == 0)
                    {
                        acccount.CharacterName = newCharacterName;
                        MessageBox.Show(string.Format("Changed character name to: {0}", newCharacterName), "Success", MessageBoxButtons.OK);
                    }
                    break;

                default:
                    throw new ArgumentException();
            }

            ConfigurationManager.Save();
            LoadAccountList();
        }

        private void ComboBoxServerSelectedIndexChanged(object sender, EventArgs e)
        {
            realCbx.Items.Clear();
            var serverInfo = (comboBoxServer.SelectedItem as ServerInfo);
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
