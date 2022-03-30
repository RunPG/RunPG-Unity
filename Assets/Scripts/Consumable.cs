using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : CombatAction
{
}

public class HealthPotion : Consumable
{
    public override string Name { get { return "Health Potion"; } }

    public override void doAction()
    {
        target.Heal(20);
    }
}

public class Bomb : Consumable
{
    public override string Name { get { return "Bomb"; } }

    public override void doAction()
    {
        target.TakeDamage(70);
    }
}