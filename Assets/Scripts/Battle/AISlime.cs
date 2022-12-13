using System.Collections.Generic;
using UnityEngine;

public class AISlime : AICharacter
{
  public override void AskForAction()
  {
    Bond bond = new Bond();
    bond.caster = this;
    List<Character> enemies = CombatManager.Instance.GetEnemies(this);
    bond.target = enemies[Random.Range(0, enemies.Count)];

    CombatManager.Instance.AddAction(bond);
  }

  protected override void InitStat(int level)
  {
    stats = new Statistics(5, 5, 10 + level * 3, 5 + level * 3, 10 + level * 3, 10 + level * 3);
  }
}
