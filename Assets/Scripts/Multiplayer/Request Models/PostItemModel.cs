namespace RunPG.Multi
{
  public class PostItemModel
  {
    public int itemId;
    public int stackSize;

    public PostItemModel(int itemId, int stackSize)
    {
      this.itemId = itemId;
      this.stackSize = stackSize;
    }

    public PostItemModel(string name, int stackSize)
    {
      this.itemId = name == "Bombe" ? 2 : 1;
      this.stackSize = stackSize;
    }
  }
}
