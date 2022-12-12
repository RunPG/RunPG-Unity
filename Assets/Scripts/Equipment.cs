using RunPG.Multi;
using UnityEngine;

public class Equipment
{
  public int id;
  public EquipmentType type;
  public string name;
  public Rarity rarity;
  public HeroClass heroClass;
  public string description;
  public int level;
  public int vitality;
  public int strength;
  public int defense;
  public int power;
  public int resistance;
  public int precision;
  public int stackSize;
  public bool isConsomable;
  public bool isItem;

  public Equipment(EquipmentModel model, int stackSize = 1)
  {
    id = model.id;
    type = model.equipmentBase.equipmentType;
    name = model.equipmentBase.name;
    rarity = model.equipmentBase.rarity;
    heroClass = model.equipmentBase.heroClass;
    description = model.equipmentBase.description;
    level = model.statistics.level;
    vitality = model.statistics.vitality;
    strength = model.statistics.strength;
    defense = model.statistics.defense;
    power = model.statistics.power;
    resistance = model.statistics.resistance;
    precision = model.statistics.precision;
    this.stackSize = stackSize;
    isConsomable = false;
    isItem = false;
  }

  public Equipment(ItemModel model, int stackSize)
  {
    id = model.id;
    name = model.name;
    description = model.description;
    isConsomable = model.isConsomable;
    this.stackSize = stackSize;
    isItem = true;

    rarity = model.id switch
    {
      < 6 => Rarity.COMMON,
      < 9 => Rarity.RARE,
      < 12 => Rarity.EPIC,
      12 => Rarity.LEGENDARY,
      _ => Rarity.COMMON
    };
  }

}

static class EquipmentMethods
{
  public static Sprite GetEquipmentSprite(this Equipment equipment)
  {
    var type = equipment.type;
    var rarity = equipment.rarity;
    var heroClass = equipment.heroClass;

    if (equipment.isItem)
    {
      return equipment.id switch
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

    return rarity switch
    {
      Rarity.COMMON => Resources.Load<Sprite>(string.Format("Inventory/Equipments/{0}/Common/{1}", heroClass, type.ToString())),
      Rarity.RARE => Resources.Load<Sprite>(string.Format("Inventory/Equipments/{0}/Common/{1}", heroClass, type.ToString())),
      Rarity.EPIC => Resources.Load<Sprite>(string.Format("Inventory/Equipments/{0}/Common/{1}", heroClass, type.ToString())),
      Rarity.LEGENDARY => Resources.Load<Sprite>(string.Format("Inventory/Equipments/{0}/Legendary/{1}", heroClass, type.ToString())),
      Rarity.RELIC => Resources.Load<Sprite>(string.Format("Inventory/Equipments/{0}/Legendary/{1}", heroClass, type.ToString())),
      _ => null,
    };
  }
}