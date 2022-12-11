using UnityEngine;

public enum Rarity
{
  COMMON,
  RARE,
  EPIC,
  LEGENDARY,
  RELIC
}

static class RarityMethods
{

  public static Color GetColor(this Rarity rarity)
  {
    return rarity switch
    {
      Rarity.COMMON => new Color(0, 255, 0),
      Rarity.RARE => new Color(0, 0, 255),
      Rarity.EPIC => new Color(102, 0, 204),
      Rarity.LEGENDARY => new Color(255, 128, 0),
      Rarity.RELIC => new Color(255, 0, 0),
      _ => new Color(0, 0, 0)
    };
  }

  public static string GetName(this Rarity rarity)
  {
    return rarity switch
    {
      Rarity.COMMON => "Commun",
      Rarity.RARE => "Rare",
      Rarity.EPIC => "Epic",
      Rarity.LEGENDARY => "Légendaire",
      Rarity.RELIC => "Relique",
      _ => ""
    };
  }

  public static Sprite GetSprite(this Rarity rarity)
  {
    return rarity switch
    {
      Rarity.COMMON => Resources.Load<Sprite>("Inventory/Rarity/Common"),
      Rarity.RARE => Resources.Load<Sprite>("Inventory/Rarity/Rare"),
      Rarity.EPIC => Resources.Load<Sprite>("Inventory/Rarity/Epic"),
      Rarity.LEGENDARY => Resources.Load<Sprite>("Inventory/Rarity/Legendary"),
      Rarity.RELIC => Resources.Load<Sprite>("Inventory/Rarity/Legendary"),
      _ => Resources.Load<Sprite>("Inventory/Rarity/Common")
    };
  }

  public static Sprite GetItemSprite(this Rarity rarity)
  {
    return rarity switch
    {
      Rarity.COMMON => Resources.Load<Sprite>("Inventory/Rarity/Common_Item"),
      Rarity.RARE => Resources.Load<Sprite>("Inventory/Rarity/Rare_Item"),
      Rarity.EPIC => Resources.Load<Sprite>("Inventory/Rarity/Epic_Item"),
      Rarity.LEGENDARY => Resources.Load<Sprite>("Inventory/Rarity/Legendary_Item"),
      Rarity.RELIC => Resources.Load<Sprite>("Inventory/Rarity/Legendary_Item"),
      _ => Resources.Load<Sprite>("Inventory/Rarity/Common_Item")
    };
  }

  public static float GetRarityMultiplier(this Rarity rarity)
  {
    return rarity switch
    {
      Rarity.COMMON => 1,
      Rarity.RARE => 1.2f,
      Rarity.EPIC => 1.5f,
      Rarity.LEGENDARY => 2f,
      _ => 1
    };
  }
}