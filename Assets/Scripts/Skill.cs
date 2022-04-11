using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : CombatAction { }


public class LightAttack : Skill
{
    public override string name => "Light Attack";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 200;

    public override void doAction()
    {
        target.TakeDamage(10);
    }
}

public class HeavyAttack : Skill
{
    public override string name => "Heavy Attack";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 100;

    public override void doAction()
    {
        target.TakeDamage(50);
    }
}
