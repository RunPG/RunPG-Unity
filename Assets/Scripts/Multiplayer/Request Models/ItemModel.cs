namespace RunPG.Multi
{
  public class ItemModel
  {
    public int id;
    public string name;
    public string description;
    public bool isConsomable;

    public ItemModel(int id, string name, string description, bool isConsomable)
    {
      this.id = id;
      this.name = name;
      this.description = description;
      this.isConsomable = isConsomable;
    }
  }
}
