using System;
using System.Collections.Generic;
using System.Linq;

namespace RunPG.Multi
{
  public class StatisticsModel
  {
    public int id;
    public int level;
    public int vitality;
    public int strength;
    public int defense;
    public int power;
    public int resistance;
    public int precision;


    public StatisticsModel(int id, int level, int vitality, int strength, int defense, int power, int resistance, int precision)
    {
      this.id = id;
      this.level = level;
      this.vitality = vitality;
      this.strength = strength;
      this.defense = defense;
      this.power = power;
      this.resistance = resistance;
      this.precision = precision;
    }

    public static StatisticsModel GenerateStatistics(int level, HeroClass? heroClass, Rarity rarity)
    {
      var weight = heroClass == HeroClass.MAGE ?
       new List<int>() { 5, 5, 5, 17, 10, 12 }
       : heroClass == HeroClass.PALADIN ?
       new List<int>() { 11, 8, 11, 8, 11, 5 }
       : new List<int>() { 10, 10, 10, 10, 10, 10 };
      var stats = new StatisticsModel(0, level, 0, 0, 0, 0, 0, 0);
      var totalWeight = weight.Sum();
      var random = new Random();

      var levelRarity = level * rarity.GetRarityMultiplier();
      levelRarity = levelRarity < 5 ? 5 : levelRarity;

      for (int i = 0; i < levelRarity; i++)
      {
        var x = random.Next(0, totalWeight);
        var tmp = 0;
        for (int j = 0; j < 6; j++)
        {
          tmp += weight[j];
          if (x < tmp)
          {
            stats.Increment(j);
            break;
          }
        }
      }
      return stats;
    }
  }

  public static class StatisticsModelMethods
  {
    public static void Increment(this StatisticsModel statisticsModel, int index)
    {
      switch (index)
      {
        case 0:
          statisticsModel.vitality++;
          break;
        case 1:
          statisticsModel.strength++;
          break;
        case 2:
          statisticsModel.defense++;
          break;
        case 3:
          statisticsModel.power++;
          break;
        case 4:
          statisticsModel.resistance++;
          break;
        case 5:
          statisticsModel.precision++;
          break;
      }
    }
  }
}
