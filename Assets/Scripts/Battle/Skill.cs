using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Skill : CombatAction 
{
    public abstract int cooldown { get; }
    public int remainingCooldownTurns { get; set; } = 0;

    public override void PlayAction()
    {
        remainingCooldownTurns = cooldown;
    }
}

public class Attendre : Skill 
{
    public override string name => "Attendre";

    public override PossibleTarget possibleTarget => PossibleTarget.Self;

    public override int speed => 0;

    public override float duration => 0.5f;

    public override int cooldown => 0;

    public override void PlayAction() { }
}

public class Entaille : Skill
{
    public override string name => "Entaille";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 200;
    public override float duration => 1f;
    public override int cooldown => 0;

    public override void PlayAction()
    {
        base.PlayAction();
        caster.PlayAnimation("Entaille");
        CombatManager.Instance.StartCoroutine(DoAction());
    }

    private IEnumerator DoAction()
    {
        yield return new WaitForSeconds(0.4f);

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

        return Mathf.RoundToInt((30 + 10 * caster.level) * attackMultiplier * critMultiplier);
    }
}

public class Provocation : Skill
{
    public override string name => "Provocation";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 300;
    public override float duration => 1.5f;

    public override int cooldown => 2;

    public override void PlayAction()
    {
        base.PlayAction();
        caster.PlayAnimation("Provocation");
        CombatManager.Instance.StartCoroutine(DoAction());
    }

    private IEnumerator DoAction()
    {
        yield return new WaitForSeconds(0.8f);

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

        return Mathf.RoundToInt((10 + 5 * caster.level) * attackMultiplier * critMultiplier);
    }
}

public class BouleDeFeu : Skill
{
    public override string name => "Boule de feu";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 100;
    public override float duration => 2f;
    public override int cooldown => 0;

    private static GameObject fireballRessource = Resources.Load<GameObject>("FireBall");

    public override void PlayAction()
    {
        base.PlayAction();
        caster.PlayAnimation("BouleDeFeu");
        CombatManager.Instance.StartCoroutine(DoAction());
    }

    private IEnumerator DoAction()
    {
        yield return new WaitForSeconds(0.8f);
        Vector3 startPos = caster.transform.Find("Wizard/Armature/Root/Spine1/Spine2/Shoulder.R/UpperArm.R/LowerArm.R/Hand.R/Hand.R_end/FireballStart").position;
        Vector3 endPos = target.transform.position;
        GameObject fireball = GameObject.Instantiate<GameObject>(fireballRessource, startPos, Quaternion.LookRotation(endPos - startPos));


        float elapsedTime = 0;
        float duration = 0.5f;
        while (elapsedTime < duration)
        {
            fireball.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.TakeDamage(GetDamage());
        CombatManager.Instance.AddStatus(new BurnStatus(), target);
        GameObject.Destroy(fireball);
    }

    private int GetDamage()
    {
        float attackMultiplier = (float)caster.stats.power / target.stats.resistance;
        float critMultiplier = 1f;

        if (caster.stats.RollCrit())
        {
            critMultiplier = caster.stats.GetCritMultiplier();
        }

        return Mathf.RoundToInt((25 + 10 * caster.level) * attackMultiplier * critMultiplier);
    }
}

public class Embrasement : Skill
{
    public override string name => "Embrasement";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 100;
    public override float duration => 2f;
    public override int cooldown => 3;

    private static GameObject igniteRessource = Resources.Load<GameObject>("Ignite");

    public override void PlayAction()
    {
        base.PlayAction();
        caster.PlayAnimation("Embrasement");
        CombatManager.Instance.StartCoroutine(DoAction());
        
    }

    private IEnumerator DoAction()
    {
        yield return new WaitForSeconds(0.65f);
        Vector3 pos = target.transform.Find("Ground").transform.position;
        GameObject ignite = GameObject.Instantiate<GameObject>(igniteRessource, pos, Quaternion.identity);
        target.TakeDamage(GetDamage());
        CombatManager.Instance.AddStatus(new BurnStatus(), target);
        yield return new WaitForSeconds(1f);
        GameObject.Destroy(ignite);
    }

    private int GetDamage()
    {
        float attackMultiplier = (float)caster.stats.power / target.stats.resistance;
        float critMultiplier = 1f;
        float burnMultiplier = target.IsAffectedByStatus("Brulure") ? 1.5f : 1f;

        if (caster.stats.RollCrit())
        {
            critMultiplier = caster.stats.GetCritMultiplier();
        }

        return Mathf.RoundToInt((20 + 10 * caster.level) * attackMultiplier * critMultiplier * burnMultiplier);
    }
}

public class Tempete : Skill
{
    public override string name => "Tempete";
    public override PossibleTarget possibleTarget => PossibleTarget.Enemy;
    public override int speed => 50;
    public override float duration => 3f;
    public override int cooldown => 0;

    private static GameObject lightningRessource = Resources.Load<GameObject>("Lightning");

    public override void PlayAction()
    {
        base.PlayAction();
        caster.PlayAnimation("Tempete");
        CombatManager.Instance.StartCoroutine(DoAction());

    }

    private IEnumerator DoAction()
    {
        yield return new WaitForSeconds(0.5f);
        List<Character> targets = CombatManager.Instance.GetAllies(target);
        for (int i = 0; i < 5; i++)
        {
            Character actualTarget;
            if (i == 0)
            {
                actualTarget = target;
            }
            else
            {
                actualTarget = targets[Random.Range(0, targets.Count)];
            }
            var pos = actualTarget.transform.Find("Ground").transform.position;
            GameObject lightning = GameObject.Instantiate(lightningRessource, pos, Quaternion.identity);
            yield return new WaitForSeconds(0.15f);
            if (Random.Range(0, 2) == 0)
            {
                CombatManager.Instance.AddStatus(new ElectrifiedStatus(), actualTarget);
            }
            actualTarget.TakeDamage(GetDamage(actualTarget));
            yield return new WaitForSeconds(0.25f);
            GameObject.Destroy(lightning);
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

        return 1;// Mathf.RoundToInt((20 + 10 * caster.level) * attackMultiplier * critMultiplier);
    }
}
