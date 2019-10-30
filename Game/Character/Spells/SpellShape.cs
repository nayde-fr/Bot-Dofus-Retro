using System.Collections.Generic;
using Bot_Dofus_1._29._1.Game.Mapas;

namespace Bot_Dofus_1._29._1.Game.Character.Spells
{
    public class SpellShape
    {

        public static IEnumerable<Cell> Get_Lista_Celdas_Rango_Hechizo(Cell celda, SpellStats spellLevel, Map mapa, int rango_adicional = 0)
        {
            int rango_maximo = spellLevel.alcanze_maximo + (spellLevel.es_alcanze_modificable ? rango_adicional : 0);
            
            if (spellLevel.es_lanzado_linea)
                return Shaper.Cruz(celda.x, celda.y, spellLevel.alcanze_minimo, rango_maximo, mapa);
            else
                return Shaper.Anillo(celda.x, celda.y, spellLevel.alcanze_minimo, rango_maximo, mapa);
        }
    }
}
