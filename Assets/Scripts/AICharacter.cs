using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacter : Character
{
    private string[] skills = new string[4];

    private void Awake()
    {
        AddConsumable("Health Potion", 1);

        skills[0] = "Light Attack";
        skills[1] = "Light Attack";
        skills[2] = "Light Attack";
        skills[3] = "Heavy Attack";

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }
    public override void AskForAction()
    {
        if (!isAlive())
        {
            CombatManager.Instance.AddAction(null);
        }
        else
        {
            if (currentHealth < 60 && HasConsumable("Health Potion"))
            {
                CombatAction action = CombatManager.Instance.GetCombatAction("Health Potion");
                action.caster = this;
                action.target = this;
                CombatManager.Instance.AddAction(action);
            }
            else
            {
                int pickSkill = Random.Range(0, skills.Length);
                CombatAction action = CombatManager.Instance.GetCombatAction(skills[pickSkill]);
                action.caster = this;
                List<Character> enemies = CombatManager.Instance.GetMyEnemies(this);
                int pickTarget = Random.Range(0, enemies.Count);
                action.target = enemies[pickTarget];
                CombatManager.Instance.AddAction(action);
            }

        }
    }
}
