using System;
using System.Collections.Generic;

public class AIDaarun : AICharacter
{
  private int turn = 0;

  static readonly List<int> potentialReward = new List<int>() { 0, 1 };

  private bool rewardDroped = false;

  public override void AskForAction()
  {
    turn++;
    if (turn % 3 == 0)
    {
      Laser laser = new Laser();
      laser.caster = this;
      List<Character> enemies = CombatManager.Instance.GetEnemies(this);
      laser.target = enemies[0];

      CombatManager.Instance.AddAction(laser);
    }
    else
    {
      QueueDeFer queueDeFer = new QueueDeFer();
      queueDeFer.caster = this;
      List<Character> enemies = CombatManager.Instance.GetEnemies(this);
      queueDeFer.target = enemies[UnityEngine.Random.Range(0, enemies.Count)];

      CombatManager.Instance.AddAction(queueDeFer);
    }
  }

  protected override System.Tuple<string, int> GetMonsterReward()
  {
    int quantity = rewardDroped ? 0 : potentialReward[UnityEngine.Random.Range(0, potentialReward.Count)];
    rewardDroped = true;
    return new Tuple<string, int>("Oeil de Daarun", quantity);
  }

  protected override void InitStat(int level)
  {
    stats = new Statistics(12 + level * 3, 12 + level * 3, 12 + level * 3, 12 + level * 3, 12 + level * 3, 12 + level * 3);
  }
}
