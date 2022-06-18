using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AICharacter : Character
{
    public void Init(string name, int maxHP)
    {
        characterName = name;
        maxHealth = maxHP;
        currentHealth = maxHP;
        healthBar.SetMaxHealth(maxHealth);
    }
}
