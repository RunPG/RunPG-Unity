using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerCharacter : Character
{
    private Canvas ActionCanvas;
    private Canvas AttackCanvas;
    private Canvas ObjectCanvas;

    private Button buttonPass;

    private Button buttonSkill1;
    private Button buttonSkill2;
    private Button buttonSkill3;
    private Button buttonSkill4;
    private Button buttonSkillBack;

    private GameObject veil1;
    private GameObject veil2;
    private GameObject veil3;
    private GameObject veil4;

    private List<Button> objectButtons = new List<Button>();
    private Button buttonObjectBack;
    private Transform objects;

    [SerializeField]
    protected string name;

    [SerializeField]
    private string[] skillNames = new string[4];

    private Skill[] skills = new Skill[4];

    private bool isSelected = false;
    private CombatAction selectedAction = null;

    protected override void Awake()
    {
        base.Awake();

        healthBarInstance.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;

        AddConsumable("Potion de vie", 2);
        AddConsumable("Bombe", 1);

        skills[0] = (Skill)CombatManager.Instance.GetCombatAction(skillNames[0]);
        skills[1] = (Skill)CombatManager.Instance.GetCombatAction(skillNames[1]);
        skills[2] = (Skill)CombatManager.Instance.GetCombatAction(skillNames[2]);
        skills[3] = (Skill)CombatManager.Instance.GetCombatAction(skillNames[3]);

        ActionCanvas = GameObject.Find("Canvas ActionSelection").GetComponent<Canvas>();
        AttackCanvas = GameObject.Find("Canvas AttackSelection").GetComponent<Canvas>();
        ObjectCanvas = GameObject.Find("Canvas ObjectSelection").GetComponent <Canvas>();

        buttonSkill1 = AttackCanvas.transform.Find("Background/Actions/Action 1/Button").GetComponent<Button>();
        buttonSkill2 = AttackCanvas.transform.Find("Background/Actions/Action 2/Button").GetComponent<Button>();
        buttonSkill3 = AttackCanvas.transform.Find("Background/Actions/Action 3/Button").GetComponent<Button>();
        buttonSkill4 = AttackCanvas.transform.Find("Background/Actions/Action 4/Button").GetComponent<Button>();

        buttonPass = ActionCanvas.transform.Find("Background/PassButton").GetComponent<Button>();

        veil1 = AttackCanvas.transform.Find("Background/Actions/Action 1/CooldownUI").gameObject;
        veil2 = AttackCanvas.transform.Find("Background/Actions/Action 2/CooldownUI").gameObject;
        veil3 = AttackCanvas.transform.Find("Background/Actions/Action 3/CooldownUI").gameObject;
        veil4 = AttackCanvas.transform.Find("Background/Actions/Action 4/CooldownUI").gameObject;

        veil1.SetActive(false);
        veil2.SetActive(false);
        veil3.SetActive(false);
        veil4.SetActive(false);

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
                        ShowSelected(false);
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

        buttonSkill1.GetComponentInChildren<Text>().text = skills[0].name;
        buttonSkill2.GetComponentInChildren<Text>().text = skills[1].name;
        buttonSkill3.GetComponentInChildren<Text>().text = skills[2].name;
        buttonSkill4.GetComponentInChildren<Text>().text = skills[3].name;

        buttonSkill1.interactable = skills[0].remainingCooldownTurns == 0;
        veil1.SetActive(skills[0].remainingCooldownTurns != 0);
        veil1.GetComponentInChildren<TextMeshProUGUI>().text = skills[0].remainingCooldownTurns.ToString() + " Tours";

        buttonSkill2.interactable = skills[1].remainingCooldownTurns == 0;
        veil2.SetActive(skills[1].remainingCooldownTurns != 0);
        veil2.GetComponentInChildren<TextMeshProUGUI>().text = skills[1].remainingCooldownTurns.ToString() + " Tours";

        buttonSkill3.interactable = skills[2].remainingCooldownTurns == 0;
        veil3.SetActive(skills[2].remainingCooldownTurns != 0);
        veil3.GetComponentInChildren<TextMeshProUGUI>().text = skills[2].remainingCooldownTurns.ToString() + " Tours";

        buttonSkill4.interactable = skills[3].remainingCooldownTurns == 0;
        veil4.SetActive(skills[3].remainingCooldownTurns != 0);
        veil4.GetComponentInChildren<TextMeshProUGUI>().text = skills[3].remainingCooldownTurns.ToString() + " Tours";

        CanvasGroup actionGroup = ActionCanvas.GetComponent<CanvasGroup>();
        actionGroup.alpha = 1;
        actionGroup.interactable = true;
        actionGroup.blocksRaycasts = true;

        isSelected = true;
        ShowSelected(true);
        selectedAction = null;

        buttonPass.onClick.AddListener(() =>
        {
            selectedAction = CombatManager.Instance.GetCombatAction("Attendre");
            selectedAction.caster = this;
            selectedAction.target = this;
            isSelected = false;
            ShowSelected(false);
            ResetInterface();
            CombatManager.Instance.HidePossibleTarget();
            CombatManager.Instance.AddAction(selectedAction);
        });

        buttonSkill1.onClick.AddListener(() =>
        {
            selectedAction = skills[0];
            selectedAction.caster = this;
            buttonSkill1.interactable = false;
            buttonSkill2.interactable = skills[1].remainingCooldownTurns == 0;
            buttonSkill3.interactable = skills[2].remainingCooldownTurns == 0;
            buttonSkill4.interactable = skills[3].remainingCooldownTurns == 0;
            CombatManager.Instance.HidePossibleTarget();
            CombatManager.Instance.ShowPossibleTarget(this, selectedAction.possibleTarget);
        });

        buttonSkill2.onClick.AddListener(() =>
        {
            selectedAction = skills[1];
            selectedAction.caster = this;
            buttonSkill1.interactable = skills[0].remainingCooldownTurns == 0;
            buttonSkill2.interactable = false;
            buttonSkill3.interactable = skills[2].remainingCooldownTurns == 0;
            buttonSkill4.interactable = skills[3].remainingCooldownTurns == 0;
            CombatManager.Instance.HidePossibleTarget();
            CombatManager.Instance.ShowPossibleTarget(this, selectedAction.possibleTarget);
        });

        buttonSkill3.onClick.AddListener(() =>
        {
            selectedAction = skills[2];
            selectedAction.caster = this;
            buttonSkill1.interactable = skills[0].remainingCooldownTurns == 0;
            buttonSkill2.interactable = skills[1].remainingCooldownTurns == 0;
            buttonSkill3.interactable = false;
            buttonSkill4.interactable = skills[3].remainingCooldownTurns == 0;
            CombatManager.Instance.HidePossibleTarget();
            CombatManager.Instance.ShowPossibleTarget(this, selectedAction.possibleTarget);
        });

        buttonSkill4.onClick.AddListener(() =>
        {
            selectedAction = skills[3];
            selectedAction.caster = this;
            buttonSkill1.interactable = skills[0].remainingCooldownTurns == 0;
            buttonSkill2.interactable = skills[1].remainingCooldownTurns == 0;
            buttonSkill3.interactable = skills[2].remainingCooldownTurns == 0;
            buttonSkill4.interactable = false;
            CombatManager.Instance.HidePossibleTarget();
            CombatManager.Instance.ShowPossibleTarget(this, selectedAction.possibleTarget);
        });

        buttonSkillBack.onClick.AddListener(() => ResetAction());

        foreach (var item in inventory)
        {
            Sprite itemSprite = CombatManager.Instance.GetItemSprite(item.Key);
            UnityAction buttonAction = () =>
            {
                selectedAction = CombatManager.Instance.GetCombatAction(item.Key);
                selectedAction.caster = this;
                foreach (var b in objectButtons)
                {
                    b.interactable = true;
                }
                var it = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                it.interactable = false;
                CombatManager.Instance.HidePossibleTarget();
                CombatManager.Instance.ShowPossibleTarget(this, selectedAction.possibleTarget);
            };

            Button button = Instantiate(objects.Find("ButtonExample").gameObject, objects, false).GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = item.Key;

            Image buttonImage = button.GetComponent<Image>();
            Image[] images = button.GetComponentsInChildren<Image>();
            foreach (var image in images)
            {
                if (image != buttonImage && itemSprite != null)
                {
                    image.sprite = itemSprite;
                    break;
                }
            }

            Text[] texts = button.GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                if (text.gameObject.name == "Name")
                    text.text = item.Key;
                else if (text.gameObject.name == "Quantity" && item.Value > 1)
                    text.text = "X" + item.Value;
            }

            button.gameObject.SetActive(true);
            button.onClick.AddListener(buttonAction);

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
        buttonPass.onClick.RemoveAllListeners();
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

    private void ShowSelected(bool status)
    {
        if (!status)
            healthBarInstance.transform.Find("Background").GetComponent<Image>().color = new Color(1f, 1f, 1f);
        else
            healthBarInstance.transform.Find("Background").GetComponent<Image>().color = new Color(0.5f, 0.7f, 1f);
    }

    public void DecreaseCooldownTurns()
    {
        foreach (var skill in skills)
        {
            skill.remainingCooldownTurns--;
            if (skill.remainingCooldownTurns < 0)
                skill.remainingCooldownTurns = 0;
        }
    }
}