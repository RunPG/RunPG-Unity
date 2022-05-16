using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : CombatAction
{
}

public class HealthPotion : Consumable
{
    public override string name => "Potion de vie";
    public override PossibleTarget possibleTarget => PossibleTarget.Self;
    public override int speed => 300;
    public override float duration => 0.5f;

    public override void PlayAction()
    {
        target.Heal(20);
    }
}

public class Bomb : Consumable
{
    public override string name => "Bombe";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 200;
    public override float duration => 0.5f;

    public override void PlayAction()
    {
        target.TakeDamage(70);
    }
}