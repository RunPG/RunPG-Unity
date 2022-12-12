using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RunPG.Multi
{
  public class EquipmentBaseModel
  {

    public int id;
    public string name;
    public string description;
    public Rarity rarity;
    public HeroClass heroClass;
    public EquipmentType equipmentType;

    public EquipmentBaseModel(int id, string name, string description, Rarity rarity, HeroClass heroClass, EquipmentType equipmentType)
    {
      this.id = id;
      this.name = name;
      this.description = description;
      this.rarity = rarity;
      this.heroClass = heroClass;
      this.equipmentType = equipmentType;
    }
  }
  static class EquipmentBaseModelMethods
  {
    public static Sprite GetEquipmentSprite(this EquipmentBaseModel equipment)
    {
      var type = equipment.equipmentType;
      var rarity = equipment.rarity;
      var heroClass = equipment.heroClass;

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
}
