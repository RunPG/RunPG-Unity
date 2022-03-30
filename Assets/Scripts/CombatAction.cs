using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatAction
{
    public abstract string Name { get; }
    public Character target { get; set; }
    public Character caster { get; set; }

    public abstract void doAction();
}
