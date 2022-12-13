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
    List<Character> enemies = CombatManager.Instance.GetMyEnemies(this);
    bond.target = enemies[UnityEngine.Random.Range(0, enemies.Count)];

    CombatManager.Instance.AddAction(bond);
  }

  protected override void InitStat(int level)
  {
    stats = new Statistics(5, 5, 10 + level * 3, 5 + level * 3, 10 + level * 3, 10 + level * 3);
  }

  protected override Tuple<string, int> GetMonsterReward()
  {
    int quantity = rewardDroped ? 0 : potentialReward[UnityEngine.Random.Range(0, potentialReward.Count)];
    rewardDroped = true;
    return new Tuple<string, int>("Bave de slime", quantity);
  }
}
