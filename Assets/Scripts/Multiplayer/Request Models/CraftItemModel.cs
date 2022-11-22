namespace RunPG.Multi
{
    public class CraftItemModel
    {
        public int id;
        public string name;
        public string description;
        public int quantity;

        public CraftItemModel(int id, string name, string description, int quantity)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.quantity = quantity;
        }
    }
}
