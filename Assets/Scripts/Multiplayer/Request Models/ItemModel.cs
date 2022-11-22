namespace RunPG.Multi
{
    public class ItemModel
    {
        public int itemId;
        public int stackSize;

        public ItemModel(int itemId, int stackSize)
        {
            this.itemId = itemId;
            this.stackSize = stackSize;
        }
    }
}
