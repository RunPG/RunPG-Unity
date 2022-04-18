using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacter : Character
{
    private string[] skills = new string[4];

    private void Awake()
    {
        AddConsumable("Potion de vie", 1);

        skills[0] = "Entaille";
        skills[1] = "Entaille";
        skills[2] = "Entaille";
        skills[3] = "Coup de bouclier";

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
                List<Character> targets = CombatManager.Instance.GetPossibleTargets(this, action.possibleTarget);
                int pickTarget = Random.Range(0, targets.Count);
                action.target = targets[pickTarget];
                CombatManager.Instance.AddAction(action);
            }

        }
    }
}
