using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    protected int remainingTurns = 0;

    public void CleanStatus()
    {
        remainingTurns = 0;
    }

    public void AddStatus(int turns)
    {
        remainingTurns += turns;
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
    public StunStatus()
    {
        remainingTurns = 1;
    }
}

public class TauntStatus : Status
{
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

public class ElementalStatus : Status {}

public class BurnStatus : ElementalStatus 
{
    public BurnStatus()
    {
        remainingTurns = 3;
    }

    public BurnStatus(int turns)
    {
        remainingTurns = turns;
    }
}

public class PoisonStatus : ElementalStatus
{
    public PoisonStatus()
    {
        remainingTurns = 6;
    }

    public PoisonStatus(int turns)
    {
        remainingTurns = turns;
    }
}

public class ElectrifiedStatus : ElementalStatus
{
    public ElectrifiedStatus()
    {
        remainingTurns = 4;
    }

    public ElectrifiedStatus(int turns)
    {
        remainingTurns = turns;
    }
}