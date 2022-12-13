using UnityEngine;

namespace RunPG.Multi
{
  public class CraftItemModel
  {
    public int id;
    public string name;
    public string description;
    public int quantity;
    public Rarity rarity;

    public CraftItemModel(int id, string name, string description, int quantity)
    {
      this.id = id;
      this.name = name;
      this.description = description;
      this.quantity = quantity;
      this.rarity = id switch
      {
        < 6 => Rarity.COMMON,
        < 9 => Rarity.RARE,
        < 12 => Rarity.EPIC,
        12 => Rarity.LEGENDARY,
        _ => Rarity.COMMON
      };
    }

    public static CraftItemModel GetFromName(string name, int quantity)
    {
      return name switch
      {
        "Oeil de Daarun" => new CraftItemModel(12, "Oeil de Daarun", "L'avoir dans sa poche peut confèrer des pouvoirs surréels.", quantity),
        _ => new CraftItemModel(13, "Bave de slime", "C'est visqueux ...", quantity),
      };
    }
  }

  static class CraftItemModelMethods
  {
    public static Sprite GetSprite(this CraftItemModel itemModel)
    {
      return itemModel.id switch
      {
        1 => Resources.Load<Sprite>("Inventory/Items/HealthPotion"),
        2 => Resources.Load<Sprite>("Inventory/Items/Bombe"),
        3 or 6 or 9 => Resources.Load<Sprite>("Inventory/Items/Stick"),
        4 or 7 or 10 => Resources.Load<Sprite>("Inventory/Items/Flower"),
        5 or 8 or 11 => Resources.Load<Sprite>("Inventory/Items/Crystal"),
        12 => Resources.Load<Sprite>("Inventory/Items/Eye"),
        13 => Resources.Load<Sprite>("Inventory/Items/Slime"),
        _ => Resources.Load<Sprite>("Inventory/Items/HealthPotion")
      };
    }
  }
}
