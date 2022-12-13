using Photon.Pun;
using RunPG.Multi;
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
  private static readonly Vector3[][] PlayerPositions = new Vector3[4][] { new Vector3[1] { new Vector3(0, 0, -0.175f) }, new Vector3[2] { new Vector3(-0.15f, 0, -0.175f), new Vector3(0.15f, 0, -0.175f) }, new Vector3[] { new Vector3(-0.2f, 0, -0.175f), new Vector3(0, 0, 0), new Vector3(0.2f, 0, -0.175f) }, new Vector3[4] { new Vector3(-0.3f, 0, 0), new Vector3(-0.15f, 0, -0.175f), new Vector3(0.15f, 0, -0.175f), new Vector3(0.3f, 0, 0) } };
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
  private GameObject daarunPrefab;

  [SerializeField]
  private List<Sprite> ItemSprites = new List<Sprite>();

  [SerializeField]
  private List<Sprite> Status;

  [SerializeField]
  private GameObject ResultScreen;
  [SerializeField]
  private Sprite LooseSprite;
  [SerializeField]
  private Sprite WinSprite;

  private List<CombatAction> queue = new List<CombatAction>();

  private DungeonManager dungeonManager;

  private Dictionary<string, int> monsterRewards = new Dictionary<string, int>();

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
    RegisterCombatAction<CoupDeBouclier>("Coup de bouclier");
    RegisterCombatAction<Chatiment>("Chatiment");
    RegisterCombatAction<BouleDeFeu>("Boule de feu");
    RegisterCombatAction<Embrasement>("Embrasement");
    RegisterCombatAction<Tempete>("Tempete");
    RegisterCombatAction<Stalactite>("Stalactite");
    RegisterCombatAction<HealthPotion>("Potion de vie");
    RegisterCombatAction<Bomb>("Bombe");
    RegisterCombatAction<Bond>("Bond");
    RegisterCombatAction<QueueDeFer>("Queue de fer");
    RegisterCombatAction<Laser>("Laser");
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
    dataToShare.Add("name", action.caster.characterName);
    dataToShare.Add("target", action.target.characterName);
    dataToShare.Add("action", action.name);
    photonView.RPC("SetAction", RpcTarget.All, dataToShare);
  }

  [PunRPC]
  void SetAction(object dataToShare)
  {
    var evt = (Dictionary<string, string>)dataToShare;
    var caster = characters.First(c => c.characterName == evt["name"]);
    var profileCharacter = caster as PlayerCharacter;
    var combatAction = combatActions[evt["action"]]();
    if (profileCharacter != null)
    {
      combatAction = profileCharacter.skills.FirstOrDefault(s => s.name == evt["action"] && s.remainingCooldownTurns == 0);
      if (combatAction == null)
        combatAction = combatActions[evt["action"]]();
    }
    combatAction.target = characters.First(c => c.characterName == evt["target"]);
    combatAction.caster = caster;
    queue.Add(combatAction);
  }


  public List<Character> GetAllies(Character ofCharacter, bool included = true)
  {
    List<Character> allies = new List<Character>();
    foreach (var character in characters)
    {
      if ((included || character != ofCharacter) && character.CompareTag(ofCharacter.tag) && character.isAlive())
        allies.Add(character);
    }
    return allies;
  }

  public List<Character> GetEnemies(Character ofCharacter)
  {
    List<Character> enemies = new List<Character>();
    foreach (var character in characters)
    {
      if (!character.CompareTag(ofCharacter.tag) && character.isAlive())
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
    var victory = true;
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
          Status status = action.caster.GetStatus().Find(s => s.name == "Etourdissement");
          ((StunStatus)status).PlayFX(action.caster);
          yield return new WaitForSeconds(1.5f);
          action.caster.DeleteStatusIcon(status);
          action.caster.GetStatus().Remove(status);
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
          if (taunt != null)
          {
            action.target = taunt.GetTaunter();
            taunt.DecraseTurns();
            if (!taunt.IsAffected())
            {
              action.caster.DeleteStatusIcon(taunt);
              action.caster.GetStatus().Remove(taunt);
            }
          }

        }

        if (action as Consumable != null)
          action.caster.UseConsumable(action.name);
        action.PlayAction();
        yield return new WaitForSeconds(action.duration);
      }

      if (ResolveBurnStatus())
        yield return new WaitForSeconds(1f);
      if (ResolveElectrifiedStatus())
        yield return new WaitForSeconds(1f);

      if (characters.Where(c => c.isAlive() && c.CompareTag("Team1")).Count() == 0)
      {
        victory = false;
        break;
      }
      else if (characters.Where(c => c.isAlive() && c.CompareTag("Team2")).Count() == 0)
      {
        victory = true;
        break;
      }
    }

    foreach (var character in DungeonManager.instance.characters)
    {
      Character c = characters.Find(c => c.characterName == character.name);
      if (c != null)
      {
        character.ratioHP = ((float)c.currentHealth) / c.maxHealth;
      }
      else
      {
        character.ratioHP = 0f;
      }
    }

    DungeonManager.instance.currentFloor += 1;

    var resultCanvasGroup = ResultScreen.GetComponent<CanvasGroup>();
    var resultTitle = resultCanvasGroup.transform.Find("Background/Header/Title").GetComponent<TextMeshProUGUI>();
    resultTitle.text = victory ? "Victoire" : "Défaite";
    resultTitle.color = victory ? Color.yellow : Color.red;

    var resultText = resultCanvasGroup.transform.Find("Background/Body/Text").GetComponent<TextMeshProUGUI>();
    var rarity = resultCanvasGroup.transform.Find("Background/Body/Rarity").GetComponent<Image>();
    var name = resultCanvasGroup.transform.Find("Background/Body/Name").GetComponent<TextMeshProUGUI>();
    var description = resultCanvasGroup.transform.Find("Background/Body/Description").GetComponent<TextMeshProUGUI>();
    if (monsterRewards.Count > 0)
    {
      resultText.gameObject.SetActive(true);
      rarity.gameObject.SetActive(true);
      name.gameObject.SetActive(true);
      description.gameObject.SetActive(true);

      var monsterReward = monsterRewards.Keys.ElementAt(0);
      var quantity = monsterRewards[monsterReward];
      var itemModel = CraftItemModel.GetFromName(monsterReward, quantity);
      var newItem = new PostItemModel(itemModel.id, quantity);

      Requests.POSTInventoryItem(PlayerProfile.id, newItem);

      rarity.sprite = itemModel.rarity.GetItemSprite();
      rarity.transform.Find("Image").GetComponent<Image>().sprite = itemModel.GetSprite();
      rarity.transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = quantity.ToString();
      name.text = itemModel.name;
      description.text = itemModel.description;
    }
    else
    {
      resultText.gameObject.SetActive(false);
      rarity.gameObject.SetActive(false);
      name.gameObject.SetActive(false);
      description.gameObject.SetActive(false);
    }

    var resultObject = resultCanvasGroup.transform.Find("Background/Body/Button");
    var resultButton = resultObject.GetComponent<Button>();
    var resultButtonImage = resultObject.GetComponent<Image>();
    var resultButtonText = resultObject.Find("Text").GetComponent<TextMeshProUGUI>();
    resultButtonImage.sprite = victory ? WinSprite : LooseSprite;
    resultButtonText.text = victory ? "Continuer" : "Quitter";
    resultButton.onClick.RemoveAllListeners();
    resultButton.onClick.AddListener(() =>
    {
      if (PhotonNetwork.IsMasterClient)
        photonView.RPC("LeaveDungeon", RpcTarget.All);
      else
        LeaveDungeon();
    });

    resultCanvasGroup.alpha = 1;
    resultCanvasGroup.blocksRaycasts = true;
    resultCanvasGroup.interactable = true;
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
      case global::Status.StatusBehaviour.Replace:
        int i = statusList.FindIndex(x => x.name == status.name);
        if (i != -1)
        {
          status.StatusObject = statusList[i].StatusObject;
          statusList[i] = status;
        }
        else
        {
          status.StatusObject = target.AddStatusIcon(status.name);
          statusList.Add(status);
        }
        break;
      case global::Status.StatusBehaviour.AddDuration:
        s = statusList.Find(x => x.name == status.name);
        if (s != null)
        {
          s.remainingTurns += status.remainingTurns;
        }
        else
        {
          status.StatusObject = target.AddStatusIcon(status.name);
          statusList.Add(status);
        }
        break;
      case global::Status.StatusBehaviour.Stack:
        status.StatusObject = target.AddStatusIcon(status.name);
        statusList.Add(status);
        break;
    }
  }

  private bool ResolveBurnStatus()
  {
    bool result = false;
    foreach (var character in characters)
    {
      if (!character.isAlive())
        continue;
      List<Status> statusList = character.GetStatus();
      for (int i = statusList.Count - 1; i >= 0; i--)
      {
        if (statusList[i].name == "Brulure")
        {
          result = true;
          character.TakeDamage(10);
          ((BurnStatus)statusList[i]).PlayFX(character);
          statusList[i].DecraseTurns();
          if (!statusList[i].IsAffected())
          {
            character.DeleteStatusIcon(statusList[i]);
            statusList.RemoveAt(i);
          }
        }
      }
    }
    return result;
  }

  private bool ResolveElectrifiedStatus()
  {
    bool result = false;
    foreach (var character in characters)
    {
      if (!character.isAlive())
        continue;
      List<Status> statusList = character.GetStatus();
      for (int i = statusList.Count - 1; i >= 0; i--)
      {
        if (statusList[i].name == "Electrocution")
        {
          List<Character> possibleTargets = GetAllies(character, false);
          if (possibleTargets.Count() > 0)
          {
            result = true;
            int x = UnityEngine.Random.Range(0, possibleTargets.Count);
            character.TakeDamage(5);
            possibleTargets[x].TakeDamage(5);
            ((ElectrifiedStatus)statusList[i]).PlayFX(character, possibleTargets[x]);
          }
          statusList[i].DecraseTurns();
          if (!statusList[i].IsAffected())
          {
            character.DeleteStatusIcon(statusList[i]);
            statusList.RemoveAt(i);
          }
        }
      }
    }
    return result;
  }

  public Sprite GetItemSprite(string itemName)
  {
    foreach (var elt in ItemSprites)
    {
      if (elt.name == itemName)
      {
        return elt;
      }
    }
    logger.LogError("CombatManager", "Error on get item sprite");

    return null;
  }

  public Sprite GetStatusSprite(string statusName)
  {
    foreach (var elt in Status)
    {
      if (elt.name == statusName)
      {
        return elt;
      }
    }

    logger.LogError("CombatManager", "Error on get status sprite");

    return null;
  }

  private void InitPlayer(int index)
  {
    if (dungeonManager.characters[index].ratioHP <= 0)
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

    playerCharacter.Init(dungeonManager.characters[index].name, dungeonManager.characters[index].level, dungeonManager.characters[index].skillNames, dungeonManager.characters[index].stats, dungeonManager.characters[index].ratioHP);

    playerCharacter.tag = "Team1";

    characters.Add(playerCharacter);
  }

  public void AddReward(Tuple<string, int> reward)
  {
    if (reward.Item2 == 0)
      return;

    if (monsterRewards.ContainsKey(reward.Item1))
    {
      monsterRewards[reward.Item1] += reward.Item2;
    }
    else
    {
      monsterRewards.Add(reward.Item1, reward.Item2);
    }
  }

  private void InitMonster(int index)
  {
    if (PhotonNetwork.IsMasterClient)
    {
      Dictionary<string, string> monster = new Dictionary<string, string>();
      monster.Add("name", dungeonManager.enemies[index].name);
      monster.Add("index", index.ToString());
      monster.Add("length", dungeonManager.enemies.Length.ToString());
      monster.Add("level", dungeonManager.enemies[index].level.ToString());
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
    var level = int.Parse(dic["level"]);

    GameObject monster = name switch
    {
      "Slime" => Instantiate(slimePrefab, EnemyPositions[length - 1][index], Quaternion.identity),
      "Daarun" => Instantiate(daarunPrefab, EnemyPositions[length - 1][index], Quaternion.identity),
      _ => throw new Exception("Monster doesn't exist: " + name),
    };

    AICharacter AICharacter = monster.GetComponent<AICharacter>();
    AICharacter.Init(name, level);

    AICharacter.tag = "Team2";

    AICharacter.characterName += index;

    Debug.Log("InitMonster name ==> " + AICharacter.name);

    characters.Add(AICharacter);
  }

  [PunRPC]
  void LeaveDungeon()
  {
    SceneManager.LoadScene("DungeonScene");
  }

}


