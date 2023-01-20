using UnityEngine;

public class Statistics
{
  public int vitality;
  public int strength;
  public int defense;
  public int power;
  public int resistance;
  public int precision;

  public Statistics(int vitality, int strength, int defense, int power, int resistance, int precision)
  {
    this.vitality = vitality;
    this.strength = strength;
    this.defense = defense;
    this.power = power;
    this.resistance = resistance;
    this.precision = precision;
  }

  public int GetMaxHp(int level)
  {
    return Mathf.RoundToInt((50 + level * 20) * (1 + 0.005f * vitality));
  }

  public bool RollCrit()
  {
    float chance = Mathf.Min(1f, 0.004f * precision);
    if (chance == 1f)
      return true;
    return Random.Range(0f, 1f) < chance;
  }

  public float GetCritMultiplier()
  {
    return Mathf.Max(2f, 2f + (precision - 250) * 0.01f);
  }
}
