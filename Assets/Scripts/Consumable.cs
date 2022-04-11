using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : CombatAction
{
}

public class HealthPotion : Consumable
{
    public override string name => "Health Potion";
    public override PossibleTarget possibleTarget => PossibleTarget.Self;
    public override int speed => 300;

    public override void doAction()
    {
        target.Heal(20);
    }
}

public class Bomb : Consumable
{
    public override string name => "Bomb";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 200;

    public override void doAction()
    {
        target.TakeDamage(70);
    }
}