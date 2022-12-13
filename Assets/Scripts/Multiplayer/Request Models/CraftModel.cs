namespace RunPG.Multi
{
  public class CraftModel
  {
    public int id;
    public EquipmentBaseModel equipementBase;
    public ItemModel item;
    public CraftItemModel[] materials;
    public int golds;

    public CraftModel(int id, EquipmentBaseModel equipementBase, CraftItemModel[] materials, int golds)
    {
      this.id = id;
      this.equipementBase = equipementBase;
      this.materials = materials;
      this.golds = golds;
    }
  }
}
