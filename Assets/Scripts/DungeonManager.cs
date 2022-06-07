using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
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

    public const int maxFloor = 3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (characters == null)
        {
            characters = new DungeonCharacterInfo[2];
            characters[0] = new DungeonCharacterInfo("yott", "Paladin", new string[4] { "Entaille", "Entaille", "Provocation", "Provocation" }, 120);
            characters[1] = new DungeonCharacterInfo("Firewop1", "Sorcier", new string[4] { "Boule de feu", "Boule de feu", "Embrasement", "Embrasement" }, 100);
        }
    }

    public void SetMonsters(DungeonMonsterInfo[] monsters)
    {
        enemies = monsters;
    }

}
