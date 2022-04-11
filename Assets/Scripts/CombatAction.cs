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

    public abstract void doAction();
}
