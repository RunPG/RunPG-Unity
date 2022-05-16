using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AICharacter : Character
{
    protected override void Awake()
    {
        base.Awake();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }
}
