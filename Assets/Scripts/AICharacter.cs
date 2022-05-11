using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AICharacter : Character
{
    private void Awake()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }
}
