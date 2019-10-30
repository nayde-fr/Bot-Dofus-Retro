namespace Bot_Dofus_1._29._1.Game.Mapas.Interactivo
{
    public class InteractiveObject
    {
        public short Gfx { get; private set; }
        public Cell Cell { get; private set; }
        public InteractiveObjectModel Model { get; private set; }
        public bool IsUsable { get; set; } = false;

        public InteractiveObject(short _gfx, Cell cell)
        {
            Gfx = _gfx;
            Cell = cell;

            InteractiveObjectModel model = InteractiveObjectModel.get_Modelo_Por_Gfx(Gfx);

            if (model != null)
            {
                Model = model;
                IsUsable = true;
            }
        }
    }
}
