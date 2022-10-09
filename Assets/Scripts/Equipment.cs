using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment
{
    public int id;
    public EquipmentType type;
    public string name;
    public Rarity rarity;
    public string heroClass;
    public string description;
    public int level;
    public int vitality;
    public int strength;
    public int defense;
    public int power;
    public int resistance;
    public int precision;

    public Equipment(EquipmentModel model)
    {
        id = model.id;
        type = model.equipementBase.equipementType;
        name = model.equipementBase.name;
        rarity = model.equipementBase.rarity;
        heroClass = model.equipementBase.heroClass;
        description = model.equipementBase.description;
        level = model.statistics.level;
        vitality = model.statistics.vitality;
        strength = model.statistics.strength;
        defense = model.statistics.defense;
        power = model.statistics.power;
        resistance = model.statistics.resistance;
        precision = model.statistics.precision;
    }

}
