using System;
using System.Reflection;
using System.Windows.Forms;
using Bot_Dofus_1._29._1.Game.Character.Jobs;
using Bot_Dofus_1._29._1.Managers;
using Bot_Dofus_1._29._1.Managers.Accounts;

namespace Bot_Dofus_1._29._1.UserInterface.Interfaces
{
    public partial class UI_Oficios : UserControl
    {
        private Account cuenta;

        public UI_Oficios()
        {
            InitializeComponent();
            set_DoubleBuffered(dataGridView_oficios);
            set_DoubleBuffered(dataGridView_skills);
        }

        public void set_Cuenta(Account _cuenta)
        {
            cuenta = _cuenta;
            cuenta.Game.Character.oficios_actualizados += personaje_Oficios_Actualizados;
        }

        private void personaje_Oficios_Actualizados()
        {
            BeginInvoke((Action)(() =>
            {
                dataGridView_oficios.Rows.Clear();
                foreach (Job oficio in cuenta.Game.Character.oficios)
                    dataGridView_oficios.Rows.Add(new object[] { oficio.id, oficio.nombre, oficio.nivel, oficio.experiencia_actual + "/" + oficio.experiencia_siguiente_nivel, oficio.get_Experiencia_Porcentaje + "%" });

                dataGridView_skills.Rows.Clear();
                foreach (JobSkills skill in cuenta.Game.Character.get_Skills_Disponibles())
                    dataGridView_skills.Rows.Add(new object[] { skill.id, skill.interactivo_modelo.nombre, skill.cantidad_minima, skill.cantidad_maxima, skill.es_craft ? skill.tiempo + "%" : skill.tiempo.ToString() });
            }));
        }

        public static void set_DoubleBuffered(Control control) => typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, control, new object[] { true });
    }
}
