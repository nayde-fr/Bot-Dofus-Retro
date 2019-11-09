using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bot_Dofus_1._29._1.Controles.ControlMapa.Celdas;
using Bot_Dofus_1._29._1.Game.Mapas;
using Bot_Dofus_1._29._1.Game.Mapas.Movimiento;
using Bot_Dofus_1._29._1.Managers.Accounts;
using Bot_Dofus_1._29._1.Managers.Movements;
using Bot_Dofus_1._29._1.UserInterface.Controles.ControlMapa;
using Bot_Dofus_1._29._1.UserInterface.Controles.ControlMapa.Animaciones;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.UserInterface.Interfaces
{
    public partial class UI_Mapa : UserControl
    {
        private Account cuenta;

        public UI_Mapa(Account _cuenta)
        {
            InitializeComponent();

            cuenta = _cuenta;
            control_mapa.set_Cuenta(cuenta);
        }

        private void UI_Mapa_Load(object sender, EventArgs e)
        {
            comboBox_calidad_minimapa.SelectedIndex = (byte)control_mapa.TipoCalidad;

            control_mapa.clic_celda += mapa_Control_Celda_Clic;
            cuenta.Game.Map.mapRefreshEvent += get_Eventos_Mapa_Cambiado;
            cuenta.Game.Map.entitiesRefreshEvent += () => control_mapa.Invalidate();
            cuenta.Game.Character.movimiento_pathfinding_minimapa += get_Dibujar_Pathfinding;
        }

        private void get_Eventos_Mapa_Cambiado()
        {
            Map mapa = cuenta.Game.Map;

            byte anchura_actual = control_mapa.mapa_anchura, altura_actual = control_mapa.mapa_altura;
            byte anchura_nueva = mapa.mapWidth, altura_nueva = mapa.mapHeight;

            if (anchura_actual != anchura_nueva || altura_actual != altura_nueva)
            {
                control_mapa.mapa_anchura = anchura_nueva;
                control_mapa.mapa_altura = altura_nueva;

                control_mapa.set_Celda_Numero();
                control_mapa.dibujar_Cuadricula();
            }

            BeginInvoke((Action)(() =>
            {
                label_mapa_id.Text = $"Position: {mapa.GetCoordinates} MapID : {mapa.mapId}";
            }));

            control_mapa.refrescar_Mapa();
        }

        private void mapa_Control_Celda_Clic(CeldaMapa celda, MouseButtons botones, bool abajo)
        {
            Map mapa = cuenta.Game.Map;
            Cell celda_actual = cuenta.Game.Character.Cell, celda_destino = mapa.GetCellFromId(celda.id);

            if (botones == MouseButtons.Left && celda_actual.cellId != 0 && celda_destino.cellId != 0 && !abajo)
            {
                switch (cuenta.Game.manager.MovementManager.get_Mover_A_Celda(celda_destino, mapa.celdas_ocupadas()))
                {
                    case MovementResult.OK:
                        cuenta.logger.log_informacion("UI_MAPA", $"Personaje desplazado a la casilla: {celda_destino.cellId}");
                    break;

                    case MovementResult.SAME_CELL:
                        cuenta.logger.log_Error("UI_MAPA", "El jugador está en la misma a la seleccionada");
                    break;

                    default:
                        cuenta.logger.log_Error("UI_MAPA", $"Error desplazando el personaje a la casilla: {celda_destino.cellId}");
                    break;
                }
            }
        }

        private void get_Dibujar_Pathfinding(List<Cell> lista_celdas) => Task.Run(() => control_mapa.agregar_Animacion(cuenta.Game.Character.Id, lista_celdas, PathFinderUtil.get_Tiempo_Desplazamiento_Mapa(lista_celdas.First(), lista_celdas), TipoAnimaciones.CHARACTER));
        private void comboBox_calidad_minimapa_SelectedIndexChanged(object sender, EventArgs e) => control_mapa.TipoCalidad = (MapQuality)comboBox_calidad_minimapa.SelectedIndex;
        private void checkBox_animaciones_CheckedChanged(object sender, EventArgs e) => control_mapa.Mostrar_Animaciones = checkBox_animaciones.Checked;
        private void checkBox_mostrar_celdas_CheckedChanged(object sender, EventArgs e) => control_mapa.Mostrar_Celdas_Id = checkBox_mostrar_celdas.Checked;
    }
}
