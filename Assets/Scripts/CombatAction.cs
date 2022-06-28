using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatAction
{
    public enum PossibleTarget
    {
        Ally,
        Self,
        Enemy,
        AllyOrSelf,
        All
    }
    public abstract string name { get; }
    public Character target { get; set; }
    public abstract PossibleTarget possibleTarget { get; }
    public Character caster { get; set; }
    public abstract int speed { get; }

    public abstract float duration { get; }

    public abstract void PlayAction();
}

public class Attendre : CombatAction
{
    public override string name => "Attendre";

    public override PossibleTarget possibleTarget => PossibleTarget.Self;

    public override int speed => 0;

    public override float duration => 0.5f;

    public override void PlayAction() {}
}

public class Bond : CombatAction
{
    public override string name => "Bond";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 300;
    public override float duration => 1.5f;

    public override void PlayAction()
    {
        caster.PlayAnimation("Bond");
        CombatManager.Instance.StartCoroutine(DoAction());
    }

    private IEnumerator DoAction()
    {
        yield return new WaitForSeconds(0.9f);
        target.TakeDamage(10);
    }
}
