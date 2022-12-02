using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
        target.TakeDamage(GetDamage());
    }

    private int GetDamage()
    {
        float attackMultiplier = (float)caster.stats.strength / target.stats.defense;
        float critMultiplier = 1f;

        if (caster.stats.RollCrit())
        {
            critMultiplier = caster.stats.GetCritMultiplier();
        }
        return Mathf.RoundToInt((10 + 4 * caster.level) * attackMultiplier * critMultiplier);
    }
}

public class Laser : CombatAction
{
    public override string name => "Laser";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 100;
    public override float duration => 2f;

    public override void PlayAction()
    {
        caster.PlayAnimation("Laser");
        CombatManager.Instance.StartCoroutine(DoAction());
    }

    private IEnumerator DoAction()
    {
        yield return new WaitForSeconds(0.2f);
        var laser = caster.transform.Find("Daarun/Armature/Bone/Bone.001/Laser/LaserEffect");
        laser.GetComponent<VisualEffect>().Play();
        yield return new WaitForSeconds(1.8f);
        List<Character> targets = CombatManager.Instance.GetAllies(target);
        foreach (var t in targets)
        {
            t.TakeDamage(GetDamage(t));
        }
    }

    private int GetDamage(Character actualTarget)
    {
        float attackMultiplier = (float)caster.stats.power / actualTarget.stats.resistance;
        float critMultiplier = 1f;

        if (caster.stats.RollCrit())
        {
            critMultiplier = caster.stats.GetCritMultiplier();
        }

        return Mathf.RoundToInt((10 + 4 * caster.level) * attackMultiplier * critMultiplier);
    }
}

public class QueueDeFer : CombatAction
{
    public override string name => "Queue de fer";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 300;
    public override float duration => 1.2f;

    public override void PlayAction()
    {
        caster.PlayAnimation("QueueDeFer");
        CombatManager.Instance.StartCoroutine(DoAction());
    }

    private IEnumerator DoAction()
    {
        yield return new WaitForSeconds(0.6f);
        target.TakeDamage(GetDamage());
    }

    private int GetDamage()
    {
        float attackMultiplier = (float)caster.stats.power / target.stats.resistance;
        float critMultiplier = 1f;

        if (caster.stats.RollCrit())
        {
            critMultiplier = caster.stats.GetCritMultiplier();
        }

        return Mathf.RoundToInt((10 + 4 * caster.level) * attackMultiplier * critMultiplier);
    }
}
