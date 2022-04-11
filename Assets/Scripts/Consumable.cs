using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : CombatAction
{
}

public class HealthPotion : Consumable
{
    public override string name { get { return "Health Potion"; } }

    public override PossibleTarget possibleTarget { get { return PossibleTarget.Self; } }

    public override void doAction()
    {
        target.Heal(20);
    }
}

public class Bomb : Consumable
{
    public override string name { get { return "Bomb"; } }

    public override PossibleTarget possibleTarget { get { return PossibleTarget.Enemy; } }

    public override void doAction()
    {
        target.TakeDamage(70);
    }
}