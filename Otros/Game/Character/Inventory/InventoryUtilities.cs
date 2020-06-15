using Bot_Dofus_1._29._1.Otros.Game.Character.Inventory.Enums;
using System.Collections.Generic;

namespace Bot_Dofus_1._29._1.Otros.Game.Character.Inventory
{
    public static class InventoryUtilities
    {
        private static Dictionary<int, List<InventorySlots>> possibles_posiciones = new Dictionary<int, List<InventorySlots>>
        {
            { 1,  new List<InventorySlots>()
            { InventorySlots.NECKLACE } },

            { 2,  new List<InventorySlots>()
            { InventorySlots.WEAPON } },

            { 3,  new List<InventorySlots>()
            { InventorySlots.WEAPON } },

            { 4,  new List<InventorySlots>()
            { InventorySlots.WEAPON } },

            { 5,  new List<InventorySlots>()
            { InventorySlots.WEAPON } },

            { 6,  new List<InventorySlots>()
            { InventorySlots.WEAPON } },

            { 7,  new List<InventorySlots>()
            { InventorySlots.WEAPON } },

            { 8,  new List<InventorySlots>()
            { InventorySlots.WEAPON } },

            { 9,  new List<InventorySlots>()
            { InventorySlots.LEFT_RING, InventorySlots.RIGHT_RING } },

            { 10,  new List<InventorySlots>()
            { InventorySlots.BELT } },

            { 11,  new List<InventorySlots>()
            { InventorySlots.BOOTS } },

            { 16,  new List<InventorySlots>()
            { InventorySlots.HAT } },

            { 17,  new List<InventorySlots>()
            { InventorySlots.CAPE } },

            { 18, new List<InventorySlots>()
            { InventorySlots.PET } },

            { 19, new List<InventorySlots>()//hacha
            { InventorySlots.WEAPON } },

            { 20, new List<InventorySlots>()//herramienta
            { InventorySlots.WEAPON } },

            { 21, new List<InventorySlots>()//pico
            { InventorySlots.WEAPON } },

            { 22, new List<InventorySlots>()//Guadaña
            { InventorySlots.WEAPON } },

            { 23, new List<InventorySlots>()
            { InventorySlots.DOFUS1, InventorySlots.DOFUS2, InventorySlots.DOFUS3, InventorySlots.DOFUS4, InventorySlots.DOFUS5, InventorySlots.DOFUS6 } },

            { 82, new List<InventorySlots>()
            { InventorySlots.SHIELD } },

            { 83, new List<InventorySlots>()//piedra de alma
            { InventorySlots.WEAPON } },
        };

        public static List<InventorySlots> get_Posibles_Posiciones(int tipo_objeto) => possibles_posiciones.ContainsKey(tipo_objeto) ? possibles_posiciones[tipo_objeto] : null;

        public static InventoryObjectsTypes get_Objetos_Inventario(byte tipo)
        {
            switch (tipo)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 19:
                case 20:
                case 21:
                case 22:
                case 102:
                case 114:
                case 9:
                case 10:
                case 11:
                case 82:
                case 99:
                case 83:
                case 16:
                case 17:
                case 81:
                case 18:
                case 23:
                case 113:
                    return InventoryObjectsTypes.EQUIPMENTS;

                case 12:
                case 13:
                case 14:
                case 25:
                case 33:
                case 42:
                case 43:
                case 44:
                case 45:
                case 49:
                case 69:
                case 70:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 79:
                case 80:
                case 85:
                case 86:
                case 87:
                case 88:
                case 89:
                case 93:
                case 94:
                case 97:
                case 100:
                case 112:
                case 115:
                case 116:
                    return InventoryObjectsTypes.MISCELLANEOUS;

                case 15:
                case 26:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 46:
                case 47:
                case 48:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                case 66:
                case 68:
                case 71:
                case 78:
                case 84:
                case 90:
                case 95:
                case 96:
                case 98:
                case 103:
                case 104:
                case 105:
                case 106:
                case 107:
                case 108:
                case 109:
                case 110:
                case 111:
                    return InventoryObjectsTypes.RESOURCES;

                case 24:
                    return InventoryObjectsTypes.QUEST_ITEMS;

                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 91:
                case 92:
                    return InventoryObjectsTypes.UNKNOWN;

                default:
                    return InventoryObjectsTypes.UNKNOWN;
            }
        }
    }
}
