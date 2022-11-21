namespace RunPG.Multi
{
    public class CraftModel
    {
        public int id;
        public EquipmentBaseModel equipementBase;
        public CraftItemModel[] materials;
        public int golds;
        public int crystals;

        public CraftModel(int id, EquipmentBaseModel equipementBase, string heroClass, CraftItemModel[] materials, int golds, int crystals)
        {
            this.id = id;
            this.equipementBase = equipementBase;
            this.materials = materials;
            this.golds = golds;
            this.crystals = crystals;
        }
    }
}
