using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    public int level { get; protected set; }

    public int maxHealth { get; protected set; }
    public int currentHealth { get; protected set; }

    public string characterName { get; set; }

    protected HealthBar healthBar;

    protected Image selector;

    [SerializeField]
    private List<Sprite> status;

    protected Dictionary<string, int> inventory = new Dictionary<string, int>();

    protected List<Status> statusList = new List<Status>();

    public Statistics stats { get; protected set; }

    [SerializeField]
    protected Animator animator;

    private Transform statusUI;

    [SerializeField]
    private Transform healthBarPosition;
    [SerializeField]
    private Transform head;

    [SerializeField]
    private GameObject healthBarGameObject;

    protected GameObject healthBarInstance;


    protected virtual void Awake()
    {
        
        GameObject HealthBarCanvas = GameObject.Find("UI/Canvas Healthbar");
        healthBarInstance = Instantiate(healthBarGameObject, HealthBarCanvas.transform).gameObject;

        healthBar = healthBarInstance.GetComponentInChildren<HealthBar>();
        selector = healthBarInstance.transform.Find("Selector").GetComponent<Image>();
        statusUI = healthBarInstance.transform.Find("StatusLayout");

        
    }

    private void Start()
    {
        GameObject HealthBarCanvas = GameObject.Find("Canvas Healthbar");
        float x = RectTransformUtility.WorldToScreenPoint(Camera.main, head.position).x;
        float y = RectTransformUtility.WorldToScreenPoint(Camera.main, healthBarPosition.position).y;

        healthBarInstance.transform.position = new Vector2(x, y);
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

    public List<Status> GetStatus()
    {
        return statusList;
    }

    public abstract void AskForAction();

    public void PlayAnimation(string animation)
    {
        animator.SetTrigger(animation + "Trigger");
    }

    public bool IsAffectedByStatus(string name)
    {
        return statusList.Find(s => s.name == name) != null;
    }

    public void CleanStun()
    {
        statusList.RemoveAll(s => s.GetType() == typeof(StunStatus));
    }

    public void AddStatusIcon(string name)
    {
        GameObject newStatusGameObject = Instantiate(statusUI.Find("StatusExample").gameObject, statusUI, false);
        newStatusGameObject.name = name;
        newStatusGameObject.SetActive(true);
        Image newStatusImage = newStatusGameObject.GetComponent<Image>();
        newStatusImage.sprite = status.Find(elt => elt.name == name);
    }

    public void DeleteStatusIcon(string name)
    {
        Destroy(statusUI.Find(name).gameObject);
    }
}