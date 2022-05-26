using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public class DungeonCharacterInfo
    {
        public DungeonCharacterInfo(string name, string classType, string[] skillNames, int maxHP, int currentHP)
        {
            this.name = name;
            this.classType = classType;
            this.skillNames = skillNames;
            this.maxHP = maxHP;
            this.currentHP = currentHP;
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

    public DungeonCharacterInfo[] characters { get; private set; }
    public DungeonMonsterInfo[] enemies { get; private set; }

    private void Start()
    {
        characters = new DungeonCharacterInfo[2];
        characters[0] = new DungeonCharacterInfo("yott", "Paladin", new string[4] { "Entaille", "Entaille", "Provocation", "Provocation" }, 120, 120);
        characters[1] = new DungeonCharacterInfo("Firewop1", "Sorcier", new string[4] { "Boule de feu", "Boule de feu", "Embrasement", "Embrasement" }, 100, 100);
    }


    public void LoadLevel()
    {
        enemies = new DungeonMonsterInfo[3];
        enemies[0] = new DungeonMonsterInfo("Slime", 50);
        enemies[1] = new DungeonMonsterInfo("Slime", 50);
        enemies[2] = new DungeonMonsterInfo("Slime", 50);

        DontDestroyOnLoad(this);
        SceneManager.LoadScene("UI");
    }

}
