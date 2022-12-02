using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDaarun : AICharacter
{
    private int turn = 0;

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
            queueDeFer.target = enemies[Random.Range(0, enemies.Count)];

            CombatManager.Instance.AddAction(queueDeFer);
        }
    }

    protected override void InitStat(int level)
    {
        stats = new Statistics(5, 5, 10 + level * 3, 5 + level * 3, 10 + level * 3, 10 + level * 3);
    }
}
