using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : CombatAction { }


public class LightAttack : Skill
{
    public override string name { get { return "Light Attack"; } }
    public override PossibleTarget possibleTarget { get { return PossibleTarget.Enemy; } }

    public override void doAction()
    {
        target.TakeDamage(10);
    }
}

public class HeavyAttack : Skill
{
    public override string name { get { return "Heavy Attack"; } }
    public override PossibleTarget possibleTarget { get{ return PossibleTarget.Enemy; } }

    public override void doAction()
    {
        target.TakeDamage(50);
    }
}
