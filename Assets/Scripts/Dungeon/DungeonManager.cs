using Photon.Pun;
using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviourPunCallbacks
{
    public class DungeonCharacterInfo
    {
        public DungeonCharacterInfo(string name, int level, string classType, string[] skillNames, Statistics stats)
        {
            this.name = name;
            this.level = level;
            this.classType = classType;
            this.skillNames = skillNames;
            this.ratioHP = 1f;
            this.stats = stats;
        }

        public string name { get; private set; }
        public int level { get; private set; }
        public string classType { get; private set; }
        public string[] skillNames { get; private set; }
        public float ratioHP;
        public Statistics stats { get; private set; }
    }

    public class DungeonMonsterInfo
    {
        public DungeonMonsterInfo(string name, int level)
        {
            this.name = name;
            this.level = level;
        }

        public string name { get; private set; }
        public int level { get; private set; }
        // level and stats when it will be needed
    }

    public static DungeonManager instance;

    public List<DungeonCharacterInfo> characters { get; private set; }
    public DungeonMonsterInfo[] enemies { get; private set; }

    public int currentFloor = 0;

    public int maxFloor;

    public List<int> path = new List<int>();
    private int seed = -1;
    public List<List<Room>> map;
    public int dungeonLevel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            var phtnView = gameObject.AddComponent<PhotonView>();
            phtnView.ViewID = 1;

            characters = new List<DungeonCharacterInfo>();

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("username", PlayerProfile.pseudo);
            string heroClass = "Sorcier";
            int level = 1;
            int vitality = 1;
            int strength = 1;
            int defense = 1;
            int power = 1;
            int resistance = 1;
            int precision = 1;
            if (PlayerProfile.characterInfo != null)
            {
                heroClass = PlayerProfile.characterInfo.heroClass.GetName();
                level = PlayerProfile.characterInfo.level;
                vitality = PlayerProfile.characterInfo.GetTotalVitality();
                strength = PlayerProfile.characterInfo.GetTotalStrength();
                defense = PlayerProfile.characterInfo.GetTotalDefense();
                power = PlayerProfile.characterInfo.GetTotalPower();
                resistance = PlayerProfile.characterInfo.GetTotalResistance();
                precision = PlayerProfile.characterInfo.GetTotalPrecision();
            }
            dic.Add("heroClass", heroClass);
            dic.Add("level", level.ToString());
            dic.Add("vitality", vitality.ToString());
            dic.Add("strength", strength.ToString());
            dic.Add("defense", defense.ToString());
            dic.Add("power", power.ToString());
            dic.Add("resistance", resistance.ToString());
            dic.Add("precision", precision.ToString());

            photonView.RPC("AddCharacter", RpcTarget.All, dic);
            path.Add(0);
            object objectSeed = System.Environment.TickCount;
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SetSeed", RpcTarget.All, objectSeed);
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Update()
    {
        //check if RPC was sent
        if (seed != -1 && map == null)
        {
            dungeonLevel = characters.Max(character => character.level);
            Debug.Log("Dungeon level: " + dungeonLevel);
            map = DungeonMap.GenerateMap(seed);
            Debug.Log("map generated");
            maxFloor = map.Count - 1;
        }
    }
    [PunRPC]
    void SetSeed(object objectSeed)
    {
        seed = (int)objectSeed;
    }
    [PunRPC]
    void SetPath(object path)
    {
        this.path = ((int[])path).ToList();
        DungeonMap.RefreshMap();
    }
    [PunRPC]
    void AddCharacter(object obj)
    {
        Dictionary<string, string> dic = (Dictionary<string, string>)obj;

        if (dic["heroClass"] == "Paladin")
            characters.Add(new DungeonCharacterInfo(dic["username"], int.Parse(dic["level"]) , "Paladin", new string[4] { "Entaille", "Provocation", "Coup de bouclier", "Attendre" },
                new Statistics(int.Parse(dic["vitality"]), int.Parse(dic["strength"]), int.Parse(dic["defense"]), int.Parse(dic["power"]), int.Parse(dic["resistance"]), int.Parse(dic["precision"]))));
        else
            characters.Add(new DungeonCharacterInfo(dic["username"], int.Parse(dic["level"]), "Sorcier", new string[4] { "Boule de feu", "Stalactite", "Embrasement", "Tempete" },
                new Statistics(int.Parse(dic["vitality"]), int.Parse(dic["strength"]), int.Parse(dic["defense"]), int.Parse(dic["power"]), int.Parse(dic["resistance"]), int.Parse(dic["precision"]))));
    }
    public void StartBattle(DungeonMonsterInfo[] monsters)
    {
        enemies = monsters;
        SceneManager.LoadScene("BattleScene");
    }


    public void GiveParty()
    {
        photonView.RPC("GiveAll", RpcTarget.All);
    }
    [PunRPC]
    async void GiveAll()
    {
        this.currentFloor++;

        var bonusCanvas = GameObject.Find("BonusPopUp");
        DungeonMap.ActiveCanvasGroup(bonusCanvas.GetComponent<CanvasGroup>());

        var equipement = (await Requests.GETEquipements())[0];

        var text = bonusCanvas.transform.Find("Background/ResultText").GetComponent<TextMeshProUGUI>();
        text.text = "Vous avez gagné:\n" + equipement.name;

        var newEquipment = new NewEquipementModel(equipement.id.ToString(), new StatisticsModel(-1, 1, 1, 1, 1, 1, 1, 1));

        await Requests.POSTInventoryEquipement(PlayerProfile.id, newEquipment);

    }


    public void HealParty()
    {
        photonView.RPC("HealAll", RpcTarget.All);
    }
    [PunRPC]
    void HealAll()
    {
        this.currentFloor++;
        foreach (DungeonCharacterInfo character in characters)
        {
            if (character.ratioHP > 0)
            {
                character.ratioHP = Mathf.Min(1f, character.ratioHP + 0.25f);
            }
        }
        DungeonMap.ActiveCanvasGroup(GameObject.Find("HealPopUp").GetComponent<CanvasGroup>());
    }

    public void HideHealMessage()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("HideHealCanvas", RpcTarget.All);
        }
    }
    [PunRPC]
    public void HideHealCanvas()
    {
        DungeonMap.DisableCanvasGroup(GameObject.Find("HealPopUp").GetComponent<CanvasGroup>());
    }

    public void HideBonusMessage()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("HideBonusCanvas", RpcTarget.All);
        }
    }
    [PunRPC]
    public void HideBonusCanvas()
    {
        DungeonMap.DisableCanvasGroup(GameObject.Find("BonusPopUp").GetComponent<CanvasGroup>());
    }

    public void AddPacours(int toIndex)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            path.Add(toIndex);
            photonView.RPC("SetPath", RpcTarget.All, path.ToArray());
            map[path.Count - 1][toIndex].onClickAction();
        }
    }
    public DungeonMonsterInfo[] generateFightEnemies(int difficulty)
    {
        DungeonManager.DungeonMonsterInfo[] roomEnemies = new DungeonManager.DungeonMonsterInfo[difficulty];
        for (int k = 0; k < difficulty; k++)
        {
            roomEnemies[k] = new DungeonManager.DungeonMonsterInfo("Slime", dungeonLevel);
        }
        return roomEnemies;
    }

    public DungeonMonsterInfo[] generateBossEnemies()
    {
        DungeonManager.DungeonMonsterInfo[] bossEnemies = new DungeonManager.DungeonMonsterInfo[1];
        bossEnemies[0] = new DungeonManager.DungeonMonsterInfo("Daarun", dungeonLevel);
        return bossEnemies;
    }

    public void SetMonsters(DungeonMonsterInfo[] monsters)
    {
        enemies = monsters;
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void LeaveDungeon()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("Leave", RpcTarget.All);
        }
        else
            Leave();
    }
    [PunRPC]
    public void Leave()
    {
        LeaveRoom();
        Destroy(gameObject);
        SceneManager.LoadScene("MapScene");
    }
}
