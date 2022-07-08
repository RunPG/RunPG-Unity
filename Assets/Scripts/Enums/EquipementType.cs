using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum EquipementType
{
    WEAPON,
    HELMET,
    CHESTPLATE,
    LEGGINGS,
    GLOVES
}

static class EquipementTypeMethods
{
    public static Sprite GetSprite(this EquipementType equipementType)
    {
        return equipementType switch
        {
            EquipementType.WEAPON => Resources.Load<Sprite>("Inventory/Sword"),
            EquipementType.HELMET => Resources.Load<Sprite>("Inventory/Helmet"),
            EquipementType.CHESTPLATE => Resources.Load<Sprite>("Inventory/Chestplate"),
            EquipementType.LEGGINGS => Resources.Load<Sprite>("Inventory/Leggings"),
            EquipementType.GLOVES => Resources.Load<Sprite>("Inventory/Gloves"),
            _ => null,
        };
    }

    public static int GetIndex(this EquipementType equipementType)
    {
        return equipementType switch
        {
            EquipementType.WEAPON => 0,
            EquipementType.HELMET => 1,
            EquipementType.CHESTPLATE => 2,
            EquipementType.LEGGINGS => 3,
            EquipementType.GLOVES => 4,
            _ => -1,
        };
    }
}