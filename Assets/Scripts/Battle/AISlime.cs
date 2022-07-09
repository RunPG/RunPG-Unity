using Photon.Pun;
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
}
