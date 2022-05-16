using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CombatManager : MonoBehaviour
{
    private ILogger logger = Debug.unityLogger;

    public static CombatManager Instance { get; private set; }

    private Dictionary<string, Func<CombatAction>> combatActions = new Dictionary<string, Func<CombatAction>>();

    [SerializeField]
    private List<Character> characters = new List<Character>();

    [SerializeField]
    private List<Sprite> Items = new List<Sprite>();

    [SerializeField]
    private GameObject ResultScreen;

    private List<CombatAction> queue = new List<CombatAction>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        RegisterCombatAction<Attendre>("Attendre");
        RegisterCombatAction<Entaille>("Entaille");
        RegisterCombatAction<Provocation>("Provocation");
        RegisterCombatAction<BouleDeFeu>("Boule de feu");
        RegisterCombatAction<Embrasement>("Embrasement");
        RegisterCombatAction<HealthPotion>("Potion de vie");
        RegisterCombatAction<Bomb>("Bombe");
    }
    private void Start()
    {
        StartCoroutine(Combat());
    }

    public void AddAction(CombatAction action)
    {
        queue.Add(action);
    }

    
    public List<Character> GetMyAllies(Character myCharacter)
    {
        List<Character> allies = new List<Character>();
        foreach (var character in characters)
        {
            if (character.CompareTag(myCharacter.tag) && character.isAlive())
                allies.Add(character);
        }
        return allies;
    }

    public List<Character> GetMyEnemies(Character myCharacter)
    {
        List<Character> enemies = new List<Character>();
        foreach (var character in characters)
        {
            if (!character.CompareTag(myCharacter.tag) && character.isAlive())
                enemies.Add(character);
        }
        return enemies;
    }

    public List<Character> GetPossibleTargets(Character caster, CombatAction.PossibleTarget possibleTarget)
    {
        List<Character> targets = new List<Character>();
        switch (possibleTarget)
        {
            case CombatAction.PossibleTarget.Self:
                targets.Add(caster);
                return targets;
            case CombatAction.PossibleTarget.Enemy:
                foreach (var character in characters)
                {
                    if (!character.CompareTag(caster.tag) && character.isAlive())
                        targets.Add(character);
                }
                return targets;
            case CombatAction.PossibleTarget.Ally:
                foreach (var character in characters)
                {
                    if (character != caster && character.CompareTag(caster.tag) && character.isAlive())
                        targets.Add(character);
                }
                return targets;
            case CombatAction.PossibleTarget.AllyOrSelf:
                foreach (var character in characters)
                {
                    if (character.CompareTag(caster.tag) && character.isAlive())
                        targets.Add(character);
                }
                return targets;
            case CombatAction.PossibleTarget.All:
                foreach (var character in characters)
                    targets.Add(character);
                return targets;
            default:
                throw new Exception("Error on possible target");
        }
    }

    public CombatAction GetCombatAction(string name)
    {
        return combatActions[name]();
    }

    private IEnumerator Combat()
    {
        while (true)
        {
            queue.Clear();
            for (int i = 0; i < characters.Count; i++)
            {
                if (!characters[i].isAlive())
                {
                    Attendre idle = new Attendre();
                    idle.target = characters[i];
                    idle.caster = characters[i];
                    queue.Add(idle);
                    continue;
                }

                if (characters[i].IsStun())
                {
                    characters[i].CleanStun();
                    Attendre idle = new Attendre();
                    idle.target = characters[i];
                    idle.caster = characters[i];
                    queue.Add(idle);
                    continue;
                }

                logger.Log("asking character " + i);
                characters[i].AskForAction();
                while (queue.Count < i + 1)
                    yield return null;
                PlayerCharacter character = characters[i] as PlayerCharacter;
                if (character != null)
                {
                    character.DecreaseCooldownTurns();
                }
            }

            foreach (var action in queue.OrderByDescending(action => action.speed))
            {
                // Later some spell may target dead characters (revive)
                if (!action.caster.isAlive() || !action.target.isAlive())
                    continue;

                if (action.caster.IsStun())
                {
                    action.caster.CleanStun();
                    continue;
                }

                if (!VerifyTarget(action))
                    throw new Exception("Error wrong target");

                logger.Log("resolving action " + action.name);

                if (action.caster.IsTaunt()
                    && (action.possibleTarget == CombatAction.PossibleTarget.Enemy
                    || action.possibleTarget == CombatAction.PossibleTarget.All)
                    && action.caster.GetTaunter().isAlive())
                {
                    action.target = action.caster.GetTaunter();
                    action.caster.DecreaseTauntTurn();
                }

                if (action as Consumable != null)
                    action.caster.UseConsumable(action.name);
                action.PlayAction();
                yield return new WaitForSeconds(action.duration);
            }

            ResolveBurnStatus();

            if (characters.Where(c => c.isAlive() && c.CompareTag("Team1")).Count() == 0)
            {
                ResultScreen.GetComponent<CanvasGroup>().alpha = 1;
                ResultScreen.GetComponentInChildren<TextMeshProUGUI>().text = "D�faite";
                ResultScreen.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                break;
            }
            else if (characters.Where(c => c.isAlive() && c.CompareTag("Team2")).Count() == 0)
            {
                ResultScreen.GetComponent<CanvasGroup>().alpha = 1;
                ResultScreen.GetComponentInChildren<TextMeshProUGUI>().text = "Victoire";
                ResultScreen.GetComponentInChildren<TextMeshProUGUI>().color = Color.yellow;
                break;
            }
        }
        logger.Log("end");
    }

    public bool VerifyTarget(CombatAction action)
    {
        return GetPossibleTargets(action.caster, action.possibleTarget).Find(character => character == action.target) != null;
    }

    public void ShowPossibleTarget(Character caster, CombatAction.PossibleTarget possibleTarget)
    {
        GetPossibleTargets(caster, possibleTarget).ForEach(target => target.ShowSelector(true));
    }

    public void HidePossibleTarget()
    {
        characters.ForEach(target => target.ShowSelector(false));
    }

    private void RegisterCombatAction<T>(string name) where T : CombatAction, new()
    {
        combatActions.Add(name, () =>
        {
            return new T();
        });
    }

    private void ResolveBurnStatus()
    {
        foreach (var character in characters)
        {
            if (character.IsAffectedByElementalStatus<BurnStatus>())
            {
                character.TakeDamage(5);
                character.DecreaseElementalStatusTurns();
            }
        }
    }

    private void ResolveElectrifiedStatus()
    {
        foreach (var character in characters)
        {
            if (character.IsAffectedByElementalStatus<BurnStatus>())
            {
                character.TakeDamage(3);
                character.DecreaseElementalStatusTurns();
                List<Character> possibleTargets = GetMyAllies(character).FindAll(c => !c.HasElementalStatus());
                if (possibleTargets.Count() > 0)
                {
                    int x = UnityEngine.Random.Range(0, possibleTargets.Count);
                    possibleTargets[x].AddElementalStatus(new ElectrifiedStatus(character.GetElementalRemainingTurns()));
                }
                character.CleanElementalStatus();
            }
        }
    }

    public Sprite GetItemSprite(string itemName)
    {
        foreach (var elt in Items)
        {
            if (elt.name == itemName)
            {
                return elt;
            }
        }
        logger.LogError("CombatManager", "Error on get item sprite");

        return null;
    }
}


