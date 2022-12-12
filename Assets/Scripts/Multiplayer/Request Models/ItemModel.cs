using UnityEngine;

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

  static class ItemModelMethods
  {
    public static Sprite GetSprite(this ItemModel itemModel)
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

    public static Rarity GetRarity(this ItemModel itemModel)
    {
      return itemModel.id switch
      {
        < 6 => Rarity.COMMON,
        < 9 => Rarity.RARE,
        < 12 => Rarity.EPIC,
        12 => Rarity.LEGENDARY,
        _ => Rarity.COMMON
      };
    }
  }
}
