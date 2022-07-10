using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CombatManager : MonoBehaviourPun
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
        RegisterCombatAction<Bond>("Bond");
    }
    private void Start()
    {
        
        dungeonManager = GameObject.FindObjectOfType<DungeonManager>();

        characters = new List<Character>();

        for (int i = 0; i < dungeonManager.characters.Count; i++)
        {
            InitPlayer(i);
        }
        if (dungeonManager.enemies != null)
        {
            for (int i = 0; i < dungeonManager.enemies.Length; i++)
            {
                InitMonster(i);
            }
        }

        StartCoroutine(Combat());
        
    }

    public void AddAction(CombatAction action)
    {
        Dictionary<string, string> dataToShare = new Dictionary<string, string>();
        dataToShare.Add("ally", "true");
        dataToShare.Add("name", action.caster.characterName);
        dataToShare.Add("target", action.target.characterName);
        dataToShare.Add("action", action.name);
        photonView.RPC("SetAction", RpcTarget.All, dataToShare);
    }

    [PunRPC]
    void SetAction(object dataToShare)
    {
        var evt = (Dictionary<string, string>)dataToShare;
        var combatAction = combatActions[evt["action"]]();
        combatAction.target = characters.First(c => c.characterName == evt["target"]);
        combatAction.caster = characters.First(c => c.characterName == evt["name"]);
        queue.Add(combatAction);
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

                if (characters[i].IsAffectedByStatus("etourdissement"))
                {
                    characters[i].CleanStun();
                    Attendre idle = new Attendre();
                    idle.target = characters[i];
                    idle.caster = characters[i];
                    queue.Add(idle);
                    continue;
                }

                logger.Log("asking character " + i);
                if (characters[i] as AICharacter && PhotonNetwork.IsMasterClient)
                {
                    characters[i].AskForAction();
                }
                else if (characters[i].characterName == PlayerProfile.pseudo)
                {
                    characters[i].AskForAction();
                }
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

                if (action.caster.IsAffectedByStatus("Etourdissement"))
                {
                    action.caster.CleanStun();
                    continue;
                }

                if (!VerifyTarget(action))
                    throw new Exception("Error wrong target");

                logger.Log("resolving action " + action.name);

                if (action.caster.IsAffectedByStatus("Provocation")
                    && (action.possibleTarget == CombatAction.PossibleTarget.Enemy
                    || action.possibleTarget == CombatAction.PossibleTarget.All))
                {
                    TauntStatus taunt = (TauntStatus)action.caster.GetStatus().Find(x => x.name == "Provocation");
                    print(taunt.remainingTurns);
                    if (taunt != null)
                    {
                        action.target = taunt.GetTaunter();
                        taunt.DecraseTurns();
                        if (!taunt.IsAffected())
                        {
                            action.caster.GetStatus().Remove(taunt);
                            action.caster.DeleteStatusIcon("Provocation");
                        }
                    }
                    
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

    public void AddStatus(Status status, Character target)
    {
        List<Status> statusList = target.GetStatus();
        Status s = null;
        switch (status.statusBehaviour)
        {
            case Status.StatusBehaviour.Replace:
                int i = statusList.FindIndex(x => x.name == status.name);
                if (i != -1)
                {
                    statusList[i] = status;
                }
                else
                {
                    statusList.Add(status);
                    target.AddStatusIcon(status.name);
                }
                break;
            case Status.StatusBehaviour.AddDuration:
                s = statusList.Find(x => x.name == status.name);
                if (s != null)
                {
                    s.remainingTurns += status.remainingTurns;
                }
                else
                {
                    statusList.Add(status);
                    target.AddStatusIcon(status.name);
                }
                break;
            case Status.StatusBehaviour.Stack:
                statusList.Add(status);
                target.AddStatusIcon(status.name);
                break;
        }
    }

    private void ResolveBurnStatus()
    {
        foreach (var character in characters)
        {
            List<Status> statusList = character.GetStatus();
            for (int i = statusList.Count - 1; i >= 0; i--)
            {
                if (statusList[i].name == "Brulure")
                {
                    character.TakeDamage(5);
                    statusList[i].DecraseTurns();
                    if (!statusList[i].IsAffected())
                    {
                        statusList.RemoveAt(i);
                        character.DeleteStatusIcon("Brulure");
                    }
                }
            }
        }
    }

    private void ResolveElectrifiedStatus()
    {
        foreach (var character in characters)
        {
            List<Status> statusList = character.GetStatus();
            for (int i = statusList.Count - 1; i >= 0; i--)
            {
                if (statusList[i].name == "Electrocution")
                {
                    character.TakeDamage(5);
                    if (statusList[i].remainingTurns > 1)
                    {
                        List<Character> possibleTargets = GetMyAllies(character);
                        if (possibleTargets.Count() > 0)
                        {
                            int x = UnityEngine.Random.Range(0, possibleTargets.Count);
                             CombatManager.Instance.AddStatus(new ElectrifiedStatus(statusList[i].remainingTurns - 1), possibleTargets[x]);
                        }
                    }
                    statusList.RemoveAt(i);
                    character.DeleteStatusIcon("Electrocution");
                }
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
                character = Instantiate(paladinPrefab, PlayerPositions[dungeonManager.characters.Count - 1][index], Quaternion.identity);
                break;
            case "Sorcier":
                character = Instantiate(wizardPrefab, PlayerPositions[dungeonManager.characters.Count - 1][index], Quaternion.identity);
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
        if (PhotonNetwork.IsMasterClient)
        {
            Dictionary<string, string> monster = new Dictionary<string, string>();
            monster.Add("name", dungeonManager.enemies[index].name);
            monster.Add("index", index.ToString());
            monster.Add("length", dungeonManager.enemies.Length.ToString());
            monster.Add("maxHp", dungeonManager.enemies[index].maxHP.ToString());
            photonView.RPC("AddMonster", RpcTarget.All, monster);
        }
    }

    [PunRPC]
    void AddMonster(object obj)
    {
        Dictionary<string, string> dic = (Dictionary<string, string>)obj;
        var name = dic["name"];
        var index = int.Parse(dic["index"]);
        var length = int.Parse(dic["length"]);
        var maxHp = int.Parse(dic["maxHp"]);
        
        GameObject monster = name switch
        {
            "Slime" => Instantiate(slimePrefab, EnemyPositions[length - 1][index], Quaternion.identity),
            _ => throw new Exception("Monster doesn't exist: " + name),
        };
        
        AICharacter AICharacter = monster.GetComponent<AICharacter>();
        AICharacter.Init(name, maxHp);

        AICharacter.tag = "Team2";

        AICharacter.characterName += index;

        Debug.Log("InitMonster name ==> " + AICharacter.name);

        characters.Add(AICharacter);
    }

}


