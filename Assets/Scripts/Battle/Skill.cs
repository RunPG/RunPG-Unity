using System.Collections;
using System.Collections.Generic;
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
        target.TakeDamage(25);
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
        CombatManager.Instance.AddStatus(new TauntStatus(caster), target);
        target.TakeDamage(20);
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

        target.TakeDamage(25);
        CombatManager.Instance.AddStatus(new BurnStatus(), target);
        GameObject.Destroy(fireball);
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
        if (target.IsAffectedByStatus("Brulure"))
        {
            target.TakeDamage(40);
        }
        else
        {
            target.TakeDamage(20);
        }
        CombatManager.Instance.AddStatus(new BurnStatus(), target);
        yield return new WaitForSeconds(1f);
        GameObject.Destroy(ignite);
    }
}
