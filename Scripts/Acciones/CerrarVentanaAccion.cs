﻿using System.Threading.Tasks;
using Bot_Dofus_1._29._1.Managers;
using Bot_Dofus_1._29._1.Managers.Accounts;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1

    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Scripts.Acciones
{
    class CerrarVentanaAccion : AccionesScript
    {
        internal override Task<ResultadosAcciones> proceso(Account cuenta)
        {
            if (cuenta.Is_In_Dialog())
            {
                cuenta.connexion.SendPacket("EV");
                return resultado_procesado;
            }

            return resultado_hecho;
        }
    }
}
