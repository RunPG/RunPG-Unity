using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : CombatAction { }


public class Entaille : Skill
{
    public override string name => "Entaille";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 200;

    public override void doAction()
    {
        caster.PlayAnimation("Entaille");
        target.TakeDamage(20);
    }
}

public class Provocation : Skill
{
    public override string name => "Provocation";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 300;

    public override void doAction()
    {
        caster.PlayAnimation("Provocation");
        target.Taunt(caster);
    }
}

public class Bond : Skill
{
    public override string name => "Bond";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 300;

    public override void doAction()
    {
        caster.PlayAnimation("Bond");
        target.TakeDamage(10);
    }
}

public class BouleDeFeu : Skill
{
    public override string name => "Boule de feu";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 100;

    public override void doAction()
    {
        caster.PlayAnimation("BouleDeFeu");
        target.TakeDamage(15);
        target.AddElementalStatus(new BurnStatus());
    }
}

public class Embrasement : Skill
{
    public override string name => "Embrasement";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 100;

    public override void doAction()
    {
        caster.PlayAnimation("Embrasement");
        target.TakeDamage(30);
        target.AddElementalStatus(new BurnStatus());
    }
}
