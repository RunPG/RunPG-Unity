using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum EquipmentType
{
    WEAPON,
    HELMET,
    CHESTPLATE,
    GLOVES,
    LEGGINGS
}

static class EquipementTypeMethods
{
    public static Sprite GetSprite(this EquipmentType equipementType)
    {
        return equipementType switch
        {
            EquipmentType.WEAPON => Resources.Load<Sprite>("Inventory/Sword"),
            EquipmentType.HELMET => Resources.Load<Sprite>("Inventory/Helmet"),
            EquipmentType.CHESTPLATE => Resources.Load<Sprite>("Inventory/Chestplate"),
            EquipmentType.LEGGINGS => Resources.Load<Sprite>("Inventory/Leggings"),
            EquipmentType.GLOVES => Resources.Load<Sprite>("Inventory/Gloves"),
            _ => null,
        };
    }

    public static int GetIndex(this EquipmentType equipementType)
    {
        return equipementType switch
        {
            EquipmentType.WEAPON => 0,
            EquipmentType.HELMET => 1,
            EquipmentType.CHESTPLATE => 2,
            EquipmentType.GLOVES => 3,
            EquipmentType.LEGGINGS => 4,
            _ => -1,
        };
    }
}