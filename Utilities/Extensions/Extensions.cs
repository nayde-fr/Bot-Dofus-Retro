using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.Linq;
using Bot_Dofus_1._29._1.Game.Enums;
using Bot_Dofus_1._29._1.Managers.Movements;

namespace Bot_Dofus_1._29._1.Utilities.Extensions
{
    public static class Extensions
    {
        public static string cadena_Amigable(this AccountState accountStatus)
        {
            switch (accountStatus)
            {
                case AccountState.CONNECTED:
                    return "Connecté";
                case AccountState.DISCONNECTED:
                    return "Deconnecté";
                case AccountState.EXCHANGE:
                    return "Echange";
                case AccountState.FIGHTING:
                    return "Combat";
                case AccountState.GATHERING:
                    return "Recolte";
                case AccountState.MOVING:
                    return "Deplacement";
                case AccountState.CONNECTED_INACTIVE:
                    return "Inactif";
                case AccountState.STORAGE:
                    return "Stockage";
                case AccountState.DIALOG:
                    return "Dialogue";
                case AccountState.BUYING:
                    return "Achat";
                case AccountState.SELLING:
                    return "Vente";
                case AccountState.REGENERATION:
                    return "Regeneration";
                default:
                    return "-";
            }
        }

        public static T get_Or<T>(this Table table, string key, DataType type, T orValue)
        {
            DynValue flag = table.Get(key);

            if (flag.IsNil() || flag.Type != type)
                return orValue;

            try
            {
                return (T)flag.ToObject(typeof(T));
            }
            catch
            {
                return orValue;
            }
        }

        public static Dictionary<MovementDirection, List<short>> Add(this Dictionary<MovementDirection, List<short>> cells, short cellId)
        {
            short[] topCells = new short[] { 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 36 };
            short[] rightCells = new short[] { 28, 57, 86, 115, 144, 173, 231, 202, 260, 289, 318, 347, 376, 405, 434 };
            short[] bottomCells = new short[] { 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463 };
            short[] leftCells = new short[] { 15, 44, 73, 102, 131, 160, 189, 218, 247, 276, 305, 334, 363, 392, 421, 450 };

            if (topCells.Contains(cellId))
            {
                if (cells.ContainsKey(MovementDirection.TOP))
                    cells[MovementDirection.TOP].Add(cellId);
                else
                {
                    cells.Add(MovementDirection.TOP, new List<short>());
                    cells[MovementDirection.TOP].Add(cellId);
                }
            }

            if (rightCells.Contains(cellId))
            {
                if (cells.ContainsKey(MovementDirection.RIGHT))
                    cells[MovementDirection.RIGHT].Add(cellId);
                else
                {
                    cells.Add(MovementDirection.RIGHT, new List<short>());
                    cells[MovementDirection.RIGHT].Add(cellId);
                }
            }

            if (bottomCells.Contains(cellId))
            {
                if (cells.ContainsKey(MovementDirection.BOTTOM))
                    cells[MovementDirection.BOTTOM].Add(cellId);
                else
                {
                    cells.Add(MovementDirection.BOTTOM, new List<short>());
                    cells[MovementDirection.BOTTOM].Add(cellId);
                }
            }

            if (leftCells.Contains(cellId))
            {
                if (cells.ContainsKey(MovementDirection.LEFT))
                    cells[MovementDirection.LEFT].Add(cellId);
                else
                {
                    cells.Add(MovementDirection.LEFT, new List<short>());
                    cells[MovementDirection.LEFT].Add(cellId);
                }
            }

            return cells;
        }
    }
}
