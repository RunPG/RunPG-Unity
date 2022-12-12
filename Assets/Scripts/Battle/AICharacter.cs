using System;
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
    protected abstract Tuple<string, int> GetMonsterReward();

    public override void TakeDamage(int damage)
    {
        if (damage < 0)
            Debug.LogWarning("damage is negative");

        currentHealth -= damage;
        Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            healthBarInstance.SetActive(false);
            CombatManager.Instance.AddReward(GetMonsterReward());
        }
        healthBar.SetHealth(currentHealth);
    }
}
