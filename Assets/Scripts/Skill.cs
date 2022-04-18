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
        target.TakeDamage(20);
    }
}

public class CoupDeBouclier : Skill
{
    public override string name => "Coup de bouclier";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 100;

    public override void doAction()
    {
        target.TakeDamage(10);
        if (Random.Range(0, 100) < 30)
        {
            Debug.Log("stun");
            target.Stun();
        }
    }
}

public class BouleDeFeu : Skill
{
    public override string name => "Boule de feu";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 100;

    public override void doAction()
    {
        target.TakeDamage(30);
        target.AddElementalStatus(new BurnStatus());
    }
}
