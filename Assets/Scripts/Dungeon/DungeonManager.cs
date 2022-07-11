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
        public DungeonCharacterInfo(string name, string classType, string[] skillNames, int maxHP)
        {
            this.name = name;
            this.classType = classType;
            this.skillNames = skillNames;
            this.maxHP = maxHP;
            this.currentHP = maxHP;
        }

        public string name { get; private set; }
        public string classType { get; private set; }
        public string[] skillNames { get; private set; }
        public int maxHP { get; private set; }
        public int currentHP { get; set; }
        // level, stats and equipement when it'll be needed
    }

    public class DungeonMonsterInfo
    {
        public DungeonMonsterInfo(string name, int maxHP)
        {
            this.name = name;
            this.maxHP = maxHP;
        }

        public string name { get; private set; }
        public int maxHP { get; private set; }
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
            var classe = "Sorcier";
            if (PlayerProfile.character != null)
            {
                classe = PlayerProfile.character.heroClass.GetName();
            }
            dic.Add("classe", classe);

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
            map = DungeonMap.GenerateMap(seed);
            Debug.Log("map generated");
            maxFloor = map.Count - 1;
        }
    }
    [PunRPC]
    void SetSeed(object objectSeed)
    {
        seed = (int) objectSeed;
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
        if (dic["classe"] == "Paladin")
            characters.Add(new DungeonCharacterInfo(dic["username"], "Paladin", new string[4] { "Entaille", "Entaille", "Provocation", "Provocation" }, 120));
        else
            characters.Add(new DungeonCharacterInfo(dic["username"], "Sorcier", new string[4] { "Boule de feu", "Boule de feu", "Embrasement", "Embrasement" }, 100));
    }
    public void StartBattle(DungeonMonsterInfo[] monsters)
    {
        enemies = monsters;
        SceneManager.LoadScene("UI");
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

        var newEquipement = new NewEquipementModel(equipement.id.ToString());

        await Requests.POSTInventoryEquipement(PlayerProfile.id, newEquipement);

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
            if (character.currentHP > 0)
            {
                int newHP = character.currentHP + (int)(0.25f * character.maxHP);
                Debug.Log("newHP: " + newHP);
                character.currentHP = newHP < character.maxHP ? newHP : character.maxHP;
                Debug.Log(character.name + ": " + character.currentHP);
                DungeonMap.ActiveCanvasGroup(GameObject.Find("HealPopUp").GetComponent<CanvasGroup>());
            }
        }
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
        DungeonMap.DisableCanvasGroup(GameObject.Find("HealPopUp").GetComponent<CanvasGroup>());
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
            roomEnemies[k] = new DungeonManager.DungeonMonsterInfo("Slime", 50);
        }
        return roomEnemies;
    }

    public DungeonMonsterInfo[] generateBossEnemies()
    {
        DungeonManager.DungeonMonsterInfo[] bossEnemies = new DungeonManager.DungeonMonsterInfo[1];
        bossEnemies[0] = new DungeonManager.DungeonMonsterInfo("Slime", 200);
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
        photonView.RPC("Leave", RpcTarget.All);
    }
    [PunRPC]
    public void Leave()
    {
        LeaveRoom();
        Destroy(gameObject);
        SceneManager.LoadScene("MapScene");
    }
}
