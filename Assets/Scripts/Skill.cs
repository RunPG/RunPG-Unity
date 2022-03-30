using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : CombatAction { }


public class LightAttack : Skill
{
    public override string Name { get { return "Light Attack"; } }

    public override void doAction()
    {
        target.TakeDamage(10);
    }
}

public class HeavyAttack : Skill
{
    public override string Name { get { return "Heavy Attack"; } }

    public override void doAction()
    {
        target.TakeDamage(50);
    }
}
