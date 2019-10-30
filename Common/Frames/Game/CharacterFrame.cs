using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bot_Dofus_1._29._1.Common.Frames.Transport;
using Bot_Dofus_1._29._1.Common.Network;
using Bot_Dofus_1._29._1.Game.Character;
using Bot_Dofus_1._29._1.Game.Character.Jobs;
using Bot_Dofus_1._29._1.Game.Enums;
using Bot_Dofus_1._29._1.Game.Mapas.Entidades;
using Bot_Dofus_1._29._1.Managers;
using Bot_Dofus_1._29._1.Managers.Accounts;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Common.Frames.Game
{
    class CharacterFrame : Frame
    {
        [Packet("As")]
        public void get_Stats_Actualizados(TcpClient cliente, string paquete) => cliente.account.game.CharacterClass.actualizar_Caracteristicas(paquete);

        [Packet("PIK")]
        public void get_Peticion_Grupo(TcpClient cliente, string paquete)
        {
            cliente.account.logger.log_informacion("Groupe", $"Nouvelle invitation de groupe du personnage: {paquete.Substring(3).Split('|')[0]}");
            cliente.SendPacket("PR");
            cliente.account.logger.log_informacion("Groupe", "Rejêt de l'invitation");
        }

        [Packet("SL")]
        public void get_Lista_Hechizos(TcpClient cliente, string paquete)
        {
            if (!paquete[2].Equals('o'))
                cliente.account.game.CharacterClass.actualizar_Hechizos(paquete.Substring(2));
        }

        [Packet("Ow")]
        public void get_Actualizacion_Pods(TcpClient cliente, string paquete)
        {
            string[] pods = paquete.Substring(2).Split('|');
            short pods_actuales = short.Parse(pods[0]);
            short pods_maximos = short.Parse(pods[1]);
            Character personaje = cliente.account.game.CharacterClass;

            personaje.inventario.pods_actuales = pods_actuales;
            personaje.inventario.pods_maximos = pods_maximos;
            cliente.account.game.CharacterClass.evento_Pods_Actualizados();
        }

        [Packet("DV")]
        public void get_Cerrar_Dialogo(TcpClient cliente, string paquete)
        {
            Account cuenta = cliente.account;

            switch (cuenta.accountState)
            {
                case AccountState.STORAGE:
                    cuenta.game.CharacterClass.inventario.evento_Almacenamiento_Abierto();
                    break;

                case AccountState.DIALOG:
                    IEnumerable<Npcs> npcs = cuenta.game.Map.lista_npcs();
                    Npcs npc = npcs.ElementAt((cuenta.game.CharacterClass.hablando_npc_id * -1) - 1);
                    npc.respuestas.Clear();
                    npc.respuestas = null;

                    cuenta.accountState = AccountState.CONNECTED_INACTIVE;
                    cuenta.game.CharacterClass.evento_Dialogo_Acabado();
                break;
            }
        }

        [Packet("EV")]
        public void get_Ventana_Cerrada(TcpClient cliente, string paquete)
        {
            Account cuenta = cliente.account;

            if (cuenta.accountState == AccountState.STORAGE)
            {
                cuenta.accountState = AccountState.CONNECTED_INACTIVE;
                cuenta.game.CharacterClass.inventario.evento_Almacenamiento_Cerrado();
            }
        }

        [Packet("JS")]
        public void get_Skills_Oficio(TcpClient cliente, string paquete)
        {
            string[] separador_skill;
            Character personaje = cliente.account.game.CharacterClass;
            Job oficio;
            JobSkills skill = null;
            short id_oficio, id_skill;
            byte cantidad_minima, cantidad_maxima;
            float tiempo;

            foreach (string datos_oficio in paquete.Substring(3).Split('|'))
            {
                id_oficio = short.Parse(datos_oficio.Split(';')[0]);
                oficio = personaje.oficios.Find(x => x.id == id_oficio);

                if (oficio == null)
                {
                    oficio = new Job(id_oficio);
                    personaje.oficios.Add(oficio);
                }

                foreach (string datos_skill in datos_oficio.Split(';')[1].Split(','))
                {
                    separador_skill = datos_skill.Split('~');
                    id_skill = short.Parse(separador_skill[0]);
                    cantidad_minima = byte.Parse(separador_skill[1]);
                    cantidad_maxima = byte.Parse(separador_skill[2]);
                    tiempo = float.Parse(separador_skill[4]);
                    skill = oficio.skills.Find(actividad => actividad.id == id_skill);

                    if (skill != null)
                        skill.set_Actualizar(id_skill, cantidad_minima, cantidad_maxima, tiempo);
                    else
                        oficio.skills.Add(new JobSkills(id_skill, cantidad_minima, cantidad_maxima, tiempo));
                }
            }

            personaje.evento_Oficios_Actualizados();
        }

        [Packet("JX")]
        public void get_Experiencia_Oficio(TcpClient cliente, string paquete)
        {
            string[] separador_oficio_experiencia = paquete.Substring(3).Split('|');
            Character personaje = cliente.account.game.CharacterClass;
            uint experiencia_actual, experiencia_base, experiencia_siguiente_nivel;
            short id;
            byte nivel;

            foreach (string oficio in separador_oficio_experiencia)
            {
                id = short.Parse(oficio.Split(';')[0]);
                nivel = byte.Parse(oficio.Split(';')[1]);
                experiencia_base = uint.Parse(oficio.Split(';')[2]);
                experiencia_actual = uint.Parse(oficio.Split(';')[3]);

                if (nivel < 100)
                    experiencia_siguiente_nivel = uint.Parse(oficio.Split(';')[4]);
                else
                    experiencia_siguiente_nivel = 0;

                personaje.oficios.Find(x => x.id == id).set_Actualizar_Oficio(nivel, experiencia_base, experiencia_actual, experiencia_siguiente_nivel);
            }
            personaje.evento_Oficios_Actualizados();
        }

        [Packet("Re")]
        public void get_Datos_Montura(TcpClient cliente, string paquete) => cliente.account.canUseMount = true;

        [Packet("OAKO")]
        public void get_Aparecer_Objeto(TcpClient cliente, string paquete) => cliente.account.game.CharacterClass.inventario.agregar_Objetos(paquete.Substring(4));

        [Packet("OR")]
        public void get_Eliminar_Objeto(TcpClient cliente, string paquete) => cliente.account.game.CharacterClass.inventario.eliminar_Objeto(uint.Parse(paquete.Substring(2)), 1, false);

        [Packet("OQ")]
        public void get_Modificar_Cantidad_Objeto(TcpClient cliente, string paquete) => cliente.account.game.CharacterClass.inventario.modificar_Objetos(paquete.Substring(2));

        [Packet("ECK")]
        public void get_Intercambio_Ventana_Abierta(TcpClient cliente, string paquete) => cliente.account.accountState = AccountState.STORAGE;

        [Packet("PCK")]
        public void get_Grupo_Aceptado(TcpClient cliente, string paquete) => cliente.account.game.CharacterClass.en_grupo = true;

        [Packet("PV")]
        public void get_Grupo_Abandonado(TcpClient cliente, string paquete) => cliente.account.game.CharacterClass.en_grupo = true;

        [Packet("ERK")]
        public void get_Peticion_Intercambio(TcpClient cliente, string paquete)
        {
            cliente.account.logger.log_informacion("INFORMATION", "L'invitation à l'échange est rejetée");
            cliente.SendPacket("EV", true);
        }

        [Packet("ILS")]
        public void get_Tiempo_Regenerado(TcpClient cliente, string paquete)
        {
            paquete = paquete.Substring(3);
            int tiempo = int.Parse(paquete);
            Account cuenta = cliente.account;
            Character personaje = cuenta.game.CharacterClass;

            personaje.timer_regeneracion.Change(Timeout.Infinite, Timeout.Infinite);
            personaje.timer_regeneracion.Change(tiempo, tiempo);

            cuenta.logger.log_informacion("DOFUS", $"Votre personnage récupère 1 pdv chaque {tiempo / 1000} secondes");
        }

        [Packet("ILF")]
        public void get_Cantidad_Vida_Regenerada(TcpClient cliente, string paquete)
        {
            paquete = paquete.Substring(3);
            int vida = int.Parse(paquete);
            Account cuenta = cliente.account;
            Character personaje = cuenta.game.CharacterClass;

            personaje.caracteristicas.vitalidad_actual += vida;
            cuenta.logger.log_informacion("DOFUS", $"Vous avez récupéré {vida} points de vie");
        }

        [Packet("eUK")]
        public void get_Emote_Recibido(TcpClient cliente, string paquete)
        {
            string[] separador = paquete.Substring(3).Split('|');
            int id = int.Parse(separador[0]), emote_id = int.Parse(separador[1]);
            Account cuenta = cliente.account;

            if (cuenta.game.CharacterClass.id != id)
                return;

            if (emote_id == 1 && cuenta.accountState != AccountState.REGENERATION)
                cuenta.accountState = AccountState.REGENERATION;
            else if (emote_id == 0 && cuenta.accountState == AccountState.REGENERATION)
                cuenta.accountState = AccountState.CONNECTED_INACTIVE;
        }

        [Packet("Bp")]
        public void get_Ping_Promedio(TcpClient cliente, string paquete) => cliente.SendPacket($"Bp{cliente.GetPingAverage()}|{cliente.GetTotalPings()}|50");

        [Packet("pong")]
        public void get_Ping_Pong(TcpClient cliente, string paquete) => cliente.account.logger.log_informacion("DOFUS", $"Ping: {cliente.GetPing()} ms");
    }
}
