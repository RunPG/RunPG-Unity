using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CombatManager : MonoBehaviour
{
    private static readonly Vector3[][] PlayerPositions = new Vector3[4][] { new Vector3[1] { new Vector3(0, 0, -0.175f) }, new Vector3[2] { new Vector3(-0.15f, 0, -0.175f), new Vector3(0.15f, 0, -0.175f) }, new Vector3[] { new Vector3(-0.2f, 0, -0.175f), new Vector3(0, 0, 0), new Vector3(0.2f, 0, -0.175f) }, new Vector3[4] {new Vector3(-0.3f, 0, 0), new Vector3(-0.15f, 0, -0.175f), new Vector3(0.15f, 0, -0.175f), new Vector3(0.3f, 0, 0) } };
    // TODO: Add positions for monsters up to 6
    private static readonly Vector3[][] EnemyPositions = new Vector3[3][] { new Vector3[1] { new Vector3(0, 0, 0.9f) }, new Vector3[2] { new Vector3(-0.25f, 0, 0.9f), new Vector3(0.25f, 0, 0.9f) }, new Vector3[3] { new Vector3(-0.3f, 0, 1.15f), new Vector3(0, 0, 0.85f), new Vector3(0.3f, 0, 1.15f) } };

    private ILogger logger = Debug.unityLogger;

    public static CombatManager Instance { get; private set; }

    private Dictionary<string, Func<CombatAction>> combatActions = new Dictionary<string, Func<CombatAction>>();


    private List<Character> characters;

    // Class
    [SerializeField]
    private GameObject wizardPrefab;
    [SerializeField]
    private GameObject paladinPrefab;


    // Enemies
    [SerializeField]
    private GameObject slimePrefab;

    [SerializeField]
    private List<Sprite> Items = new List<Sprite>();

    [SerializeField]
    private GameObject ResultScreen;

    private List<CombatAction> queue = new List<CombatAction>();

    private DungeonManager dungeonManager;

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
        
        dungeonManager = GameObject.FindObjectOfType<DungeonManager>();

        characters = new List<Character>();

        for (int i = 0; i < dungeonManager.characters.Length; i++)
        {
            InitPlayer(i);
        }

        for (int i = 0; i < dungeonManager.enemies.Length; i++)
        {
            InitMonster(i);
        }

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
                ResultScreen.GetComponentInChildren<TextMeshProUGUI>().text = "Défaite";
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

        foreach (var character in DungeonManager.instance.characters)
        {
            Character c = characters.Find(c => c.characterName == character.name);
            if (c != null)
            {
                character.currentHP = c.currentHealth;
            }
            else
            {
                character.currentHP = 0;
            }
        }

        DungeonManager.instance.currentFloor += 1;

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("DungeonScene");
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

    private void InitPlayer(int index)
    {
        if (dungeonManager.characters[index].currentHP <= 0)
            return;
        GameObject character;
        switch (dungeonManager.characters[index].classType)
        {
            case "Paladin":
                character = Instantiate(paladinPrefab, PlayerPositions[dungeonManager.characters.Length - 1][index], Quaternion.identity);
                break;
            case "Sorcier":
                character = Instantiate(wizardPrefab, PlayerPositions[dungeonManager.characters.Length - 1][index], Quaternion.identity);
                break;
            default:
                throw new Exception("Class doesn't exist: " + dungeonManager.characters[index].classType);
        }

        PlayerCharacter playerCharacter = character.GetComponent<PlayerCharacter>();

        playerCharacter.Init(dungeonManager.characters[index].name, dungeonManager.characters[index].skillNames, dungeonManager.characters[index].maxHP, dungeonManager.characters[index].currentHP);

        playerCharacter.tag = "Team1";

        characters.Add(playerCharacter);
    }

    private void InitMonster(int index)
    {
        GameObject monster;
        switch (dungeonManager.enemies[index].name)
        {
            case "Slime":
                monster = Instantiate(slimePrefab, EnemyPositions[dungeonManager.enemies.Length - 1][index], Quaternion.identity);
                break;
            default:
                throw new Exception("Monster doesn't exist: " + dungeonManager.enemies[index].name);
        }

        AICharacter AICharacter = monster.GetComponent<AICharacter>();
        AICharacter.Init(dungeonManager.enemies[index].name, dungeonManager.enemies[index].maxHP);

        AICharacter.tag = "Team2";

        characters.Add(AICharacter);
    }
}


