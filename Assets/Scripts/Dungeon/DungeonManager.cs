using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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

    public DungeonCharacterInfo[] characters { get; private set; }
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
            characters = new DungeonCharacterInfo[2];
            characters[0] = new DungeonCharacterInfo("yott", "Paladin", new string[4] { "Entaille", "Entaille", "Provocation", "Provocation" }, 120);
            characters[1] = new DungeonCharacterInfo("Firewop1", "Sorcier", new string[4] { "Boule de feu", "Boule de feu", "Embrasement", "Embrasement" }, 100);
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
    public void StartBattle(DungeonMonsterInfo[] monsters)
    {
        enemies = monsters;
        SceneManager.LoadScene("UI");
    }

    public void HealParty()
    {
        foreach (DungeonCharacterInfo character in characters)
        {
            if (character.currentHP > 0)
            {
                int newHP = character.currentHP + (int)(0.25f * character.maxHP);
                Debug.Log("newHP: " + newHP);
                character.currentHP = newHP < character.maxHP ? newHP : character.maxHP;
                Debug.Log(character.name + ": " + character.currentHP);
            }
        }
    }
    public void AddPacours(int toIndex)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            path.Add(toIndex);
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
}
