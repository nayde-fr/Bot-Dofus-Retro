using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Bot_Dofus_1._29._1.Managers.Accounts;
using Bot_Dofus_1._29._1.Managers.Groups;
using Bot_Dofus_1._29._1.UserInterface.Controles.TabControl;
using Bot_Dofus_1._29._1.UserInterface.Interfaces;
using Bot_Dofus_1._29._1.Utilities.Config;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
	Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.UserInterface.Forms
{
    public partial class Principal : Form
    {
        public static Dictionary<string, Pagina> LoadedAccounts;

        public Principal()
        {
            InitializeComponent();
            LoadedAccounts = new Dictionary<string, Pagina>();

            Directory.CreateDirectory("mapas");
            Directory.CreateDirectory("items");
        }

        private void gestionDeCuentasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AccountManagement gestion_cuentas = new AccountManagement())
            {
                if (gestion_cuentas.ShowDialog() == DialogResult.OK)
                {
                    List<AccountConfig> cuentas_para_cargar = gestion_cuentas.GetLoadedAccounts();

                    if (cuentas_para_cargar.Count < 2)
                    {
                        AccountConfig cuenta_conf = cuentas_para_cargar[0];
                        LoadedAccounts.Add(cuenta_conf.Username, agregar_Nueva_Tab_Pagina(cuenta_conf.Username, new UI_Principal(new Account(cuenta_conf)), "Ninguno"));
                    }
                    else
                    {
                        AccountConfig configuracion_lider = cuentas_para_cargar.First();
                        Account lider = new Account(configuracion_lider);
                        GroupManager grupo = new GroupManager(lider);
                        LoadedAccounts.Add(configuracion_lider.Username, agregar_Nueva_Tab_Pagina(configuracion_lider.Username, new UI_Principal(lider), configuracion_lider.Username));
                        cuentas_para_cargar.Remove(configuracion_lider);

                        foreach (AccountConfig cuenta_conf in cuentas_para_cargar)
                        {
                            Account cuenta = new Account(cuenta_conf);

                            grupo.agregar_Miembro(cuenta);
                            LoadedAccounts.Add(cuenta_conf.Username, agregar_Nueva_Tab_Pagina(cuenta_conf.Username, new UI_Principal(cuenta), grupo.lider.Configuration.Username));
                        }
                    }
                }
            }
        }

        private Pagina agregar_Nueva_Tab_Pagina(string titulo, UserControl control, string nombre_grupo)
        {
            Pagina nueva_pagina = tabControlCuentas.agregar_Nueva_Pagina(titulo);
            nueva_pagina.cabezera.propiedad_Imagen = Properties.Resources.circulo_rojo;
            nueva_pagina.cabezera.propiedad_Estado = "Desconectado";
            nueva_pagina.cabezera.propiedad_Grupo = nombre_grupo;
            nueva_pagina.contenido.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            return nueva_pagina;
        }

        private void opcionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Opciones form_opciones = new Opciones())
                form_opciones.ShowDialog();
        }
    }
}
