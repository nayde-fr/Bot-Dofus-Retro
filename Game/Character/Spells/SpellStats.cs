﻿using System.Collections.Generic;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Game.Character.Spells
{
    public class SpellStats
    {
        public byte coste_pa { get; set; }
        public byte alcanze_minimo { get; set; }
        public byte alcanze_maximo { get; set; }

        public bool es_lanzado_linea { get; set; }
        public bool es_lanzado_con_vision { get; set; }
        public bool es_celda_vacia { get; set; }
        public bool es_alcanze_modificable { get; set; }

        public byte lanzamientos_por_turno { get; set; }
        public byte lanzamientos_por_objetivo { get; set; }
        public byte intervalo { get; set; }

        public List<SpellEffect> efectos_normales { get; private set; }
        public List<SpellEffect> efectos_criticos { get; private set; }

        public SpellStats()
        {
            efectos_normales = new List<SpellEffect>();
            efectos_criticos = new List<SpellEffect>();
        }

        public void agregar_efecto(SpellEffect effect, bool es_critico)
        {
            if (!es_critico)
                efectos_normales.Add(effect);
            else
                efectos_criticos.Add(effect);
        }
    }
}
