using System;
using System.Drawing;
using System.Windows.Forms;
using Bot_Dofus_1._29._1.Managers;
using Bot_Dofus_1._29._1.Managers.Accounts;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.UserInterface.Interfaces
{
    public partial class UI_Personaje : UserControl
    {
        private Account cuenta;

        public UI_Personaje(Account _cuenta)
        {
            InitializeComponent();
            cuenta = _cuenta;

            ui_hechizos.set_Cuenta(cuenta);
            ui_oficios.set_Cuenta(cuenta);

            cuenta.game.CharacterClass.personaje_seleccionado += personaje_Seleccionado_Servidor_Juego;
            cuenta.game.CharacterClass.caracteristicas_actualizadas += personaje_Caracteristicas_Actualizadas;
         }

        private void personaje_Seleccionado_Servidor_Juego()
        {
            BeginInvoke((Action)(() =>
            {
                Bitmap imagen_raza = Properties.Resources.ResourceManager.GetObject("_" + cuenta.game.CharacterClass.raza_id + cuenta.game.CharacterClass.sexo) as Bitmap;
                imagen_personaje.Image = imagen_raza;

                label_nombre_personaje.Text = cuenta.game.CharacterClass.nombre;
            }));
        }

        private void personaje_Caracteristicas_Actualizadas()
        {
            BeginInvoke((Action)(() =>
            {
                //Sumario
                label_puntos_vida.Text = cuenta.game.CharacterClass.caracteristicas.vitalidad_actual.ToString() + '/' + cuenta.game.CharacterClass.caracteristicas.vitalidad_maxima.ToString();
                label_puntos_accion.Text = cuenta.game.CharacterClass.caracteristicas.puntos_accion.total_stats.ToString();
                label_puntos_movimiento.Text = cuenta.game.CharacterClass.caracteristicas.puntos_movimiento.total_stats.ToString();
                label_iniciativa.Text = cuenta.game.CharacterClass.caracteristicas.iniciativa.total_stats.ToString();
                label_prospeccion.Text = cuenta.game.CharacterClass.caracteristicas.prospeccion.total_stats.ToString();
                label_alcanze.Text = cuenta.game.CharacterClass.caracteristicas.alcanze.total_stats.ToString();
                label_invocaciones.Text = cuenta.game.CharacterClass.caracteristicas.criaturas_invocables.total_stats.ToString();

                //Caracteristicas
                stats_vitalidad.Text = cuenta.game.CharacterClass.caracteristicas.vitalidad.base_personaje.ToString() + " (" + cuenta.game.CharacterClass.caracteristicas.vitalidad.equipamiento.ToString() + ")";
                stats_sabiduria.Text = cuenta.game.CharacterClass.caracteristicas.sabiduria.base_personaje.ToString() + " (" + cuenta.game.CharacterClass.caracteristicas.sabiduria.equipamiento.ToString() + ")";
                stats_fuerza.Text = cuenta.game.CharacterClass.caracteristicas.fuerza.base_personaje.ToString() + " (" + cuenta.game.CharacterClass.caracteristicas.fuerza.equipamiento.ToString() + ")";
                stats_inteligencia.Text = cuenta.game.CharacterClass.caracteristicas.inteligencia.base_personaje.ToString() + " (" + cuenta.game.CharacterClass.caracteristicas.inteligencia.equipamiento.ToString() + ")";
                stats_suerte.Text = cuenta.game.CharacterClass.caracteristicas.suerte.base_personaje.ToString() + " (" + cuenta.game.CharacterClass.caracteristicas.suerte.equipamiento.ToString() + ")";
                stats_agilidad.Text = cuenta.game.CharacterClass.caracteristicas.agilidad.base_personaje.ToString() + " (" + cuenta.game.CharacterClass.caracteristicas.agilidad.equipamiento.ToString() + ")";

                //Otros
                label_capital_stats.Text = cuenta.game.CharacterClass.puntos_caracteristicas.ToString();
                label_nivel_personaje.Text = $"Nivel {cuenta.game.CharacterClass.nivel}";
            }));
        }
    }
}
