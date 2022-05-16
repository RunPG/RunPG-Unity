using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    protected int maxHealth = 100;
    protected int currentHealth;

    protected HealthBar healthBar;

    protected Image selector;

    [SerializeField]
    private List<Sprite> status;

    protected Dictionary<string, int> inventory = new Dictionary<string, int>();

    protected ElementalStatus elementalStatus;
    protected StunStatus stunStatus;
    protected TauntStatus tauntStatus;

    [SerializeField]
    protected Animator animator;

    private Transform statusUI;

    [SerializeField]
    private Transform healthBarPosition;

    [SerializeField]
    private GameObject healthBarGameObject;

    private GameObject healthBarInstance;


    protected virtual void Awake()
    {
        
        GameObject HealthBarCanvas = GameObject.Find("Canvas Healthbar");
        healthBarInstance = Instantiate(healthBarGameObject, HealthBarCanvas.transform).gameObject;

        healthBar = healthBarInstance.GetComponentInChildren<HealthBar>();
        selector = healthBarInstance.transform.Find("Selector").GetComponent<Image>();
        statusUI = healthBarInstance.transform.Find("StatusLayout");

        healthBarInstance.transform.position = Camera.main.WorldToScreenPoint(healthBarPosition.position);
    }

    public bool isAlive()
    {
        return (currentHealth > 0);
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0)
            Debug.LogWarning("damage is negative");

        currentHealth -= damage;
        Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            healthBarInstance.SetActive(false);
        }
        healthBar.SetHealth(currentHealth);
    }

    public void Heal(int heal)
    {
        if (heal < 0)
            Debug.LogWarning("heal is negative");

        currentHealth += heal;
        Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
    }

    public void AddConsumable(string name, int quantity)
    {
        if (quantity < 1)
            return;

        if (CombatManager.Instance.GetCombatAction(name) == null)
            return;
        

        if (inventory.ContainsKey(name))
            inventory[name] += quantity;
        else
            inventory.Add(name, quantity);
    }
    public void UseConsumable(string name)
    {
        try
        {
            inventory[name]--;

            if (inventory[name] == 0)
                inventory.Remove(name);
        }
        catch (Exception)
        {
            Debug.LogError("Error while using item: " + name);
        }
    }

    public bool HasConsumable(string name)
    {
        return inventory.ContainsKey(name) && inventory[name] > 0;
    }

    public void ShowSelector(bool status)
    {
        selector.enabled = status;
    }

    public void CleanElementalStatus()
    {
        elementalStatus = null;
    }

    public void AddElementalStatus(ElementalStatus status)
    {
        if (elementalStatus == null)
        {
            elementalStatus = status;
            AddStatusIcon(status.ToString());
        }
    }

    public void DecreaseElementalStatusTurns()
    {
        if (elementalStatus != null)
        {
            elementalStatus.DecraseTurns();
            if (!elementalStatus.IsAffected())
            {
                DeleteStatusIcon(elementalStatus.ToString());
                elementalStatus = null;
            }
        }
    }

    public bool HasElementalStatus()
    {
        return elementalStatus != null;
    }

    public bool IsAffectedByElementalStatus<T>() where T : ElementalStatus
    {
        if (elementalStatus == null)
            return false;
        else
            return elementalStatus is T;
    }

    public int GetElementalRemainingTurns()
    {
        if (!HasElementalStatus())
            return 0;
        else
            return elementalStatus.GetRemainingTurns();
    }

    public void CleanStun()
    {
        stunStatus = null;
        DeleteStatusIcon("StunStatus");
    }

    public void Stun()
    {
        stunStatus = new StunStatus();
        AddStatusIcon("StunStatus");
    }

    public void CleanTaunt()
    {
        tauntStatus = null;
        DeleteStatusIcon("TauntStatus");
    }

    public void Taunt(Character caster)
    {
        tauntStatus = new TauntStatus(caster);
        AddStatusIcon("TauntStatus");
    }

    public Character GetTaunter()
    {
        if (tauntStatus != null)
        {
            return tauntStatus.GetTaunter();
        }
        return null;
    }

    public void DecreaseTauntTurn()
    {
        if (tauntStatus != null)
        {
            tauntStatus.DecraseTurns();
            if (tauntStatus.GetRemainingTurns() <= 0)
            {
                tauntStatus = null;
                DeleteStatusIcon("TauntStatus");
            }
        }
    }

    public bool IsStun()
    {
        return stunStatus != null;
    }

    public bool IsTaunt()
    {
        return tauntStatus != null;
    }

    public abstract void AskForAction();

    public void PlayAnimation(string animation)
    {
        animator.SetTrigger(animation + "Trigger");
    }

    private void AddStatusIcon(string name)
    {
        GameObject newStatusGameObject = Instantiate(statusUI.Find("StatusExample").gameObject, statusUI, false);
        newStatusGameObject.name = name;
        newStatusGameObject.SetActive(true);
        Image newStatusImage = newStatusGameObject.GetComponent<Image>();
        newStatusImage.sprite = status.Find(elt => elt.name == name);
    }

    private void DeleteStatusIcon(string name)
    {
        Destroy(statusUI.Find(name).gameObject);
    }
}