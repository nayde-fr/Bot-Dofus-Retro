using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using Bot_Dofus_1._29._1.Utilities.Config;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
	Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.UserInterface.Forms
{
    public partial class Opciones : Form
    {
        public Opciones()
        {
            InitializeComponent();

            checkBox_mensajes_debug.Checked = ConfigurationManager.ShouldDebug();
        }

        private void boton_opciones_guardar_Click(object sender, EventArgs e)
        {
            ConfigurationManager.Configuration.DebugPackets = checkBox_mensajes_debug.Checked;
            ConfigurationManager.Save();
            Close();
        }

    }
}
