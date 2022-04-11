using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : Character
{
    private Canvas ActionCanvas;
    private Canvas AttackCanvas;
    private Canvas ObjectCanvas;

    private Button buttonSkill1;
    private Button buttonSkill2;
    private Button buttonSkill3;
    private Button buttonSkill4;
    private Button buttonSkillBack;

    private List<Button> objectButtons = new List<Button>();
    private Button buttonObjectBack;
    private Transform objects;

    private string[] skills = new string[4];

    private bool isSelected = false;
    private CombatAction selectedAction = null;

    void Awake()
    {
        AddConsumable("Health Potion", 1);
        AddConsumable("Bomb", 1);

        ActionCanvas = GameObject.Find("Canvas ActionSelection").GetComponent<Canvas>();
        AttackCanvas = GameObject.Find("Canvas AttackSelection").GetComponent<Canvas>();
        ObjectCanvas = GameObject.Find("Canvas ObjectSelection").GetComponent <Canvas>();

        skills[0] = "Light Attack";
        skills[1] = "Light Attack";
        skills[2] = "Heavy Attack";
        skills[3] = "Heavy Attack";

        buttonSkill1 = AttackCanvas.transform.Find("Background/Actions/Button Action 1").GetComponent<Button>();
        buttonSkill2 = AttackCanvas.transform.Find("Background/Actions/Button Action 2").GetComponent<Button>();
        buttonSkill3 = AttackCanvas.transform.Find("Background/Actions/Button Action 3").GetComponent<Button>();
        buttonSkill4 = AttackCanvas.transform.Find("Background/Actions/Button Action 4").GetComponent<Button>();
        buttonSkillBack = AttackCanvas.transform.Find("Background/Button Back").GetComponent<Button>();

        objects = ObjectCanvas.transform.Find("Background/Scroll/ScrollBack/Objects");
        buttonObjectBack = ObjectCanvas.transform.Find("Background/Button Back").GetComponent<Button>();

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (isSelected && selectedAction != null && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (raycastHit.transform.gameObject.GetComponent<Character>() != null)
                {
                    selectedAction.target = raycastHit.transform.gameObject.GetComponent<Character>();
                    if (CombatManager.Instance.VerifyTarget(selectedAction))
                    {
                        isSelected = false;
                        ResetInterface();
                        CombatManager.Instance.HidePossibleTarget();
                        CombatManager.Instance.AddAction(selectedAction);
                    }
                }
            }
        }
    }
    public override void AskForAction()
    {
        if (!isAlive())
        {
            CombatManager.Instance.AddAction(null);
            return;
        }

        buttonSkill1.GetComponentInChildren<Text>().text = skills[0];
        buttonSkill2.GetComponentInChildren<Text>().text = skills[1];
        buttonSkill3.GetComponentInChildren<Text>().text = skills[2];
        buttonSkill4.GetComponentInChildren<Text>().text = skills[3];
        CanvasGroup actionGroup = ActionCanvas.GetComponent<CanvasGroup>();
        actionGroup.alpha = 1;
        actionGroup.interactable = true;
        actionGroup.blocksRaycasts = true;

        isSelected = true;
        selectedAction = null;

        buttonSkill1.onClick.AddListener(() =>
        {
            selectedAction = CombatManager.Instance.GetCombatAction(skills[0]);
            selectedAction.caster = this;
            buttonSkill1.interactable = false;
            buttonSkill2.interactable = true;
            buttonSkill3.interactable = true;
            buttonSkill4.interactable = true;
            CombatManager.Instance.HidePossibleTarget();
            CombatManager.Instance.ShowPossibleTarget(this, selectedAction.possibleTarget);
        });

        buttonSkill2.onClick.AddListener(() =>
        {
            selectedAction = CombatManager.Instance.GetCombatAction(skills[1]);
            selectedAction.caster = this;
            buttonSkill1.interactable = true;
            buttonSkill2.interactable = false;
            buttonSkill3.interactable = true;
            buttonSkill4.interactable = true;
            CombatManager.Instance.HidePossibleTarget();
            CombatManager.Instance.ShowPossibleTarget(this, selectedAction.possibleTarget);
        });

        buttonSkill3.onClick.AddListener(() =>
        {
            selectedAction = CombatManager.Instance.GetCombatAction(skills[2]);
            selectedAction.caster = this;
            buttonSkill1.interactable = true;
            buttonSkill2.interactable = true;
            buttonSkill3.interactable = false;
            buttonSkill4.interactable = true;
            CombatManager.Instance.HidePossibleTarget();
            CombatManager.Instance.ShowPossibleTarget(this, selectedAction.possibleTarget);
        });

        buttonSkill4.onClick.AddListener(() =>
        {
            selectedAction = CombatManager.Instance.GetCombatAction(skills[3]);
            selectedAction.caster = this;
            buttonSkill1.interactable = true;
            buttonSkill2.interactable = true;
            buttonSkill3.interactable = true;
            buttonSkill4.interactable = false;
            CombatManager.Instance.HidePossibleTarget();
            CombatManager.Instance.ShowPossibleTarget(this, selectedAction.possibleTarget);
        });

        buttonSkillBack.onClick.AddListener(() => ResetAction());

        foreach (var item in inventory)
        {
            Button button = Instantiate(objects.Find("ButtonExample").gameObject, objects, false).GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = item.Key;
            button.gameObject.SetActive(true);
            button.onClick.AddListener(() =>
            {
                selectedAction = CombatManager.Instance.GetCombatAction(item.Key);
                selectedAction.caster = this;
                foreach(var b in objectButtons)
                {
                    b.interactable = true;
                }
                button.interactable= false;
                CombatManager.Instance.HidePossibleTarget();
                CombatManager.Instance.ShowPossibleTarget(this, selectedAction.possibleTarget);
            });
            objectButtons.Add(button);
        }

        buttonObjectBack.onClick.AddListener(() => ResetAction());
    }

    public void ResetAction()
    {
        selectedAction = null;
        buttonSkill1.interactable = true;
        buttonSkill2.interactable = true;
        buttonSkill3.interactable = true;
        buttonSkill4.interactable = true;
        foreach (var b in objectButtons)
        {
            b.interactable = true;
        }
        CombatManager.Instance.HidePossibleTarget();
    }

    private void ResetInterface()
    {
        foreach (var b in objectButtons)
        {
            Destroy(b.gameObject);
        }
        objectButtons.Clear();
        buttonSkill1.onClick.RemoveAllListeners();
        buttonSkill2.onClick.RemoveAllListeners();
        buttonSkill3.onClick.RemoveAllListeners();
        buttonSkill4.onClick.RemoveAllListeners();
        buttonSkillBack.onClick.RemoveAllListeners();
        buttonObjectBack.onClick.RemoveAllListeners();
        buttonSkill1.interactable = true;
        buttonSkill2.interactable = true;
        buttonSkill3.interactable = true;
        buttonSkill4.interactable = true;
        CanvasGroup attackGroup = AttackCanvas.GetComponent<CanvasGroup>();
        attackGroup.alpha = 0;
        attackGroup.interactable = false;
        attackGroup.blocksRaycasts = false;
        CanvasGroup actionGroup = ActionCanvas.GetComponent<CanvasGroup>();
        actionGroup.alpha = 0;
        actionGroup.interactable = false;
        actionGroup.blocksRaycasts = false;
        CanvasGroup objectGroup = ObjectCanvas.GetComponent<CanvasGroup>();
        objectGroup.alpha = 0;
        objectGroup.interactable = false;
        objectGroup.blocksRaycasts = false;
    }
}