using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Status
{
    public enum StatusBehaviour
    {
        Replace,
        Stack,
        AddDuration
    }

    public abstract StatusBehaviour statusBehaviour { get; }
    public abstract string name { get; }

    public int remainingTurns = 0;

    public GameObject StatusObject;

   public void CleanStatus()
    {
        remainingTurns = 0;
    }

    public void DecraseTurns()
    {
        remainingTurns--;
    }

    public bool IsAffected()
    {
        return remainingTurns > 0;
    }

    public int GetRemainingTurns()
    {
        return remainingTurns;
    }
}

public class StunStatus : Status
{
    public override StatusBehaviour statusBehaviour => StatusBehaviour.Replace;
    public override string name => "Etourdissement";

    public StunStatus()
    {
        remainingTurns = 1;
    }
}

public class TauntStatus : Status
{
    public override StatusBehaviour statusBehaviour => StatusBehaviour.Replace;
    public override string name => "Provocation";

    protected Character taunter;
    public TauntStatus(Character caster)
    {
        taunter = caster;
        remainingTurns = 3;
    }

    public Character GetTaunter()
    {
        return taunter;
    }
}

public class BurnStatus : Status 
{
    public override StatusBehaviour statusBehaviour => StatusBehaviour.AddDuration;
    public override string name => "Brulure";

    public BurnStatus()
    {
        remainingTurns = 3;
    }

    public BurnStatus(int turns)
    {
        remainingTurns = turns;
    }
}

public class ElectrifiedStatus : Status
{
    public override StatusBehaviour statusBehaviour => StatusBehaviour.Stack;
    public override string name => "Electrocution";

    private static GameObject electricArcRessource = Resources.Load<GameObject>("ElectricArc");

    public ElectrifiedStatus()
    {
        remainingTurns = 1;
    }

    public ElectrifiedStatus(int turns)
    {
        remainingTurns = turns;
    }

    public void PlayFX(Character mainTarget, Character secondTarget)
    {
        CombatManager.Instance.StartCoroutine(FXCoroutine(mainTarget, secondTarget));
    }

    private IEnumerator FXCoroutine(Character mainTarget, Character secondTarget)
    {
        GameObject electricArc = GameObject.Instantiate(electricArcRessource, Vector3.zero, Quaternion.identity);
        Vector3 start = mainTarget.transform.Find("Head").position;
        Vector3 end = secondTarget.transform.Find("Head").position;
        Vector3 direction = end - start;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
        electricArc.transform.Find("pos 1").position = start;
        electricArc.transform.Find("pos 2").position = Vector3.Lerp(start, end, 0.25f) + (Quaternion.AngleAxis(Random.Range(0, 359), direction.normalized) * (0.1f * perpendicular));
        electricArc.transform.Find("pos 3").position = Vector3.Lerp(start, end, 0.75f) + (Quaternion.AngleAxis(Random.Range(0, 359), direction.normalized) * (0.1f * perpendicular));
        electricArc.transform.Find("pos 4").position = end;
        electricArc.GetComponentInChildren<VisualEffect>().Play();
        yield return new WaitForSeconds(0.2f);
        GameObject.Destroy(electricArc);
    }
}