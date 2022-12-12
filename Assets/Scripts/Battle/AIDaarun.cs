using System;
using System.Collections.Generic;

public class AIDaarun : AICharacter
{
  private int turn = 0;

  static readonly List<int> potentialReward = new List<int>() { 0, 1 };

  public override void AskForAction()
  {
    turn++;
    if (turn % 3 == 0)
    {
      Laser laser = new Laser();
      laser.caster = this;
      List<Character> enemies = CombatManager.Instance.GetMyEnemies(this);
      laser.target = enemies[0];

      CombatManager.Instance.AddAction(laser);
    }
    else
    {
      QueueDeFer queueDeFer = new QueueDeFer();
      queueDeFer.caster = this;
      List<Character> enemies = CombatManager.Instance.GetMyEnemies(this);
      queueDeFer.target = enemies[UnityEngine.Random.Range(0, enemies.Count)];

      CombatManager.Instance.AddAction(queueDeFer);
    }
  }

  protected override System.Tuple<string, int> GetMonsterReward()
  {
    int quantity = potentialReward[UnityEngine.Random.Range(0, potentialReward.Count)];
    return new Tuple<string, int>("Bave de slime", quantity);
  }

  protected override void InitStat(int level)
  {
    stats = new Statistics(5, 5, 10 + level * 3, 5 + level * 3, 10 + level * 3, 10 + level * 3);
  }
}
