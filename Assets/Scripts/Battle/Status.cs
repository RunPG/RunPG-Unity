using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public ElectrifiedStatus()
    {
        remainingTurns = 3;
    }

    public ElectrifiedStatus(int turns)
    {
        remainingTurns = turns;
    }
}