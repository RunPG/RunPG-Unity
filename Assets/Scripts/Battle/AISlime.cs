using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISlime : AICharacter
{
    public override void AskForAction()
    {
        Bond bond = new Bond();
        bond.caster = this;
        List<Character> enemies = CombatManager.Instance.GetMyEnemies(this);
        bond.target = enemies[Random.Range(0, enemies.Count)];

        CombatManager.Instance.AddAction(bond);
    }

    protected override void InitStat(int level)
    {
        stats.power = 5;
        stats.precision = 5;
        stats.vitality = 10 + level * 3;
        stats.strength = 5 + level * 3;
        stats.defense = 10 + level * 3;
        stats.resistance = 10 + level * 3;
    }
}
