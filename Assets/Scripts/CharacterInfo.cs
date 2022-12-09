using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum HeroClass
{
    MAGE,
    BERSERKER,
    PRIEST,
    ROGUE,
    PALADIN
}

static class HeroClassMethods
{
    public static string GetName(this HeroClass heroClass)
    {
        return heroClass switch
        {
            HeroClass.MAGE => "Sorcier",
            HeroClass.BERSERKER => "Berserk",
            HeroClass.PRIEST => "Pr�tre",
            HeroClass.ROGUE => "Assassin",
            HeroClass.PALADIN => "Paladin",
            _ => "",
        };
    }

    public static Sprite GetSprite(this HeroClass heroclass)
    {
        return heroclass switch
        {
            HeroClass.MAGE => Resources.Load<Sprite>("Classe/Sorcier"),
            HeroClass.BERSERKER => Resources.Load<Sprite>("Classe/Berserk"),
            HeroClass.PRIEST => Resources.Load<Sprite>("Classe/Pretre"),
            HeroClass.ROGUE => Resources.Load<Sprite>("Classe/Assassin"),
            HeroClass.PALADIN => Resources.Load<Sprite>("Classe/Paladin"),
            _ => null,
        };
    }
}

public class CharacterInfo
{
    public HeroClass heroClass;
    public int level;
    public int experience;
    public int statisticsId;
    public int vitality;
    public int strength;
    public int defense;
    public int power;
    public int resistance;
    public int precision;
    public string spell1;
    public string spell2;
    public string spell3;
    public string spell4;
    public Equipment weapon;
    public Equipment helmet;
    public Equipment chestplate;
    public Equipment gloves;
    public Equipment leggings;
    public int gold;
    public int calories;

    public int GetTotalVitality()
    {
        return vitality + (weapon?.vitality ?? 0) + (helmet?.vitality ?? 0) + (chestplate?.vitality ?? 0) + (gloves?.vitality ?? 0) + (leggings?.vitality ?? 0);
    }

    public int GetTotalStrength()
    {
        return strength + (weapon?.strength ?? 0) + (helmet?.strength ?? 0) + (chestplate?.strength ?? 0) + (gloves?.strength ?? 0) + (leggings?.strength ?? 0);
    }

    public int GetTotalDefense()
    {
        return defense + (weapon?.defense ?? 0) + (helmet?.defense ?? 0) + (chestplate?.defense ?? 0) + (gloves?.defense ?? 0) + (leggings?.defense ?? 0);
    }

    public int GetTotalPower()
    {
        return power + (weapon?.power ?? 0) + (helmet?.power ?? 0) + (chestplate?.power ?? 0) + (gloves?.power ?? 0) + (leggings?.power ?? 0);
    }

    public int GetTotalResistance()
    {
        return resistance + (weapon?.resistance ?? 0) + (helmet?.resistance ?? 0) + (chestplate?.resistance ?? 0) + (gloves?.resistance ?? 0) + (leggings?.resistance ?? 0);
    }

    public int GetTotalPrecision()
    {
        return precision + (weapon?.precision ?? 0) + (helmet?.precision ?? 0) + (chestplate?.precision ?? 0) + (gloves?.precision ?? 0) + (leggings?.precision ?? 0);
    }

    public int GetRequiredExperience()
    {
        return 500 * level;
    }

    public int GetRequiredExperience(int level)
    {
        return 500 * level;
    }

    public async static Task<CharacterInfo> Load(int id)
    {
        CharacterInfo characterInfo = new CharacterInfo();
        var userCharacterModel = await Requests.GETUserCharacter(id);

        characterInfo.heroClass = userCharacterModel.character.heroClass;
        characterInfo.level = userCharacterModel.statistics.level;
        characterInfo.experience = userCharacterModel.character.experience;
        characterInfo.statisticsId = userCharacterModel.statistics.id;
        characterInfo.vitality = userCharacterModel.statistics.vitality;
        characterInfo.strength = userCharacterModel.statistics.strength;
        characterInfo.defense = userCharacterModel.statistics.defense;
        characterInfo.power = userCharacterModel.statistics.power;
        characterInfo.resistance = userCharacterModel.statistics.resistance;
        characterInfo.precision = userCharacterModel.statistics.precision;

        var weaponTask = Requests.GETEquipmentById(userCharacterModel.character.weaponId);
        var helmetTask = Requests.GETEquipmentById(userCharacterModel.character.helmetId);
        var chestplateTask = Requests.GETEquipmentById(userCharacterModel.character.chestplateId);
        var glovesTask = Requests.GETEquipmentById(userCharacterModel.character.glovesId);
        var leggingsTask = Requests.GETEquipmentById(userCharacterModel.character.leggingsId);

        var weaponModel = await weaponTask;
        characterInfo.weapon = weaponModel != null ? new Equipment(weaponModel) : null;
        var helmetModel = await helmetTask;
        characterInfo.helmet = helmetModel != null ? new Equipment(helmetModel) : null;
        var chestplateModel = await chestplateTask;
        characterInfo.chestplate = chestplateModel != null ? new Equipment(chestplateModel) : null;
        var glovesModel = await glovesTask;
        characterInfo.gloves = glovesModel != null ? new Equipment(glovesModel) : null;
        var leggingsModels = await leggingsTask;
        characterInfo.leggings = leggingsModels != null ? new Equipment(leggingsModels) : null;

        characterInfo.gold = userCharacterModel.character.gold;
        var caloriesModel = await Requests.GetCaloriesToday(id);
        if (caloriesModel != null)
            characterInfo.calories = caloriesModel.calories;

        return characterInfo;
    }

    public async Task updateHeaderInformations(int id)
    {
        var userCharacterModel = await Requests.GETUserCharacter(id);
        this.experience = userCharacterModel.character.experience;
        this.gold = userCharacterModel.character.gold;
        var caloriesModel = await Requests.GetCaloriesToday(id);
        if (caloriesModel != null)
            this.calories = caloriesModel.calories;

    }
}
