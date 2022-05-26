using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AICharacter : Character
{
    protected string monsterName;

    public void Init(string name, int maxHP)
    {
        monsterName = name;
        maxHealth = maxHP;
        currentHealth = maxHP;
        healthBar.SetMaxHealth(maxHealth);
    }
}
