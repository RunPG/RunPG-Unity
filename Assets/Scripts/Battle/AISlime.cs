using System;
using System.Collections.Generic;

public class AISlime : AICharacter
{
  static readonly List<int> potentialReward = new List<int>() { 0, 0, 0, 0, 0, 1, 1, 1, 1, 2 };
  private bool rewardDroped = false;

  public override void AskForAction()
  {
    Bond bond = new Bond();
    bond.caster = this;
    List<Character> enemies = CombatManager.Instance.GetEnemies(this);
    bond.target = enemies[UnityEngine.Random.Range(0, enemies.Count)];

    CombatManager.Instance.AddAction(bond);
  }

  protected override void InitStat(int level)
  {
    stats = new Statistics(10 + level * 5, 5 + level * 3, 8 + level * 3, 4, 8 + level * 3, 5);
  }

  protected override Tuple<string, int> GetMonsterReward()
  {
    int quantity = 1;
    rewardDroped = true;
    return new Tuple<string, int>("Bave de slime", quantity);
  }
}
