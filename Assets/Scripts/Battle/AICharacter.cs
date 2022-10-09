using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AICharacter : Character
{
    public void Init(string name, int level)
    {
        characterName = name;
        this.level = level;
        InitStat(level);
        maxHealth = this.stats.GetMaxHp(this.level);
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    protected abstract void InitStat(int level);
}
