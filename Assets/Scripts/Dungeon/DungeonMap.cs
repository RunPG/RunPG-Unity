using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonMap : MonoBehaviour
{
    [SerializeField]
    public FlexibleGridLayout flexibleGrid;
    [SerializeField]
    private Canvas LeavePopup;

    private void Start()
    {
        bool alive = false;
        bool? victory = null;

        if (DungeonManager.instance.currentFloor == 0)
            StartCoroutine(flexibleGrid.AutoScroll(2f, false));

        foreach (var character in DungeonManager.instance.characters)
        {
            if (character.currentHP > 0)
                alive = true;
        }
        if (!alive)
        {
            flexibleGrid.createDefeatText();
            StartCoroutine(flexibleGrid.AutoScroll(2f));
            victory = false;
        }
        else if (DungeonManager.instance.currentFloor == DungeonManager.instance.maxFloor)
        {
            flexibleGrid.createVictoryText();
            StartCoroutine(flexibleGrid.AutoScroll(2f));
            victory = true;
        }
        flexibleGrid.generateMap(victory);
    }

    public static List<List<Room.RoomType>> GenerateMap(int seed)
    {
        Random.InitState(seed);
        int dungeonSize = Random.Range(4, 8);
        List<List<Room.RoomType>> map = new List<List<Room.RoomType>>(dungeonSize);
        map.Add(new List<Room.RoomType>() { Room.RoomType.Start });
        for (int i = 0; i < dungeonSize - 1; i++)
        {
            int rowSize = Random.Range(1, 4);
            List<Room.RoomType> row = new List<Room.RoomType>(rowSize);
            for (int j = 0; j < rowSize; j++)
            {
                var r = Random.Range(0, 10);
                switch (r)
                {
                    case < 2:
                        row.Add(Room.RoomType.Heal);
                        break;

                    case < 4:
                        row.Add(Room.RoomType.Random);
                        break;

                    case < 6:
                        row.Add(Room.RoomType.Bonus);
                        break;

                    default:
                        row.Add(Room.RoomType.Fight);
                        break;
                }
            }
            map.Add(row);
        }
        map.Add(new List<Room.RoomType>() { Room.RoomType.Boss });
        return map;
    }

    public static void LoadLevel(int level)
    {
        DungeonManager.DungeonMonsterInfo[] enemies = null;
        switch (level)
        {
            case 1:
                enemies = new DungeonManager.DungeonMonsterInfo[1];
                enemies[0] = new DungeonManager.DungeonMonsterInfo("Slime", 50);
                break;
            case 2:
                enemies = new DungeonManager.DungeonMonsterInfo[2];
                enemies[0] = new DungeonManager.DungeonMonsterInfo("Slime", 50);
                enemies[1] = new DungeonManager.DungeonMonsterInfo("Slime", 50);
                break;
            case 3:
                enemies = new DungeonManager.DungeonMonsterInfo[3];
                enemies[0] = new DungeonManager.DungeonMonsterInfo("Slime", 50);
                enemies[1] = new DungeonManager.DungeonMonsterInfo("Slime", 50);
                enemies[2] = new DungeonManager.DungeonMonsterInfo("Slime", 50);
                break;
            default:
                enemies = new DungeonManager.DungeonMonsterInfo[1];
                enemies[0] = new DungeonManager.DungeonMonsterInfo("Slime", 50);
                break;
        }

        DungeonManager.instance.SetMonsters(enemies);

        SceneManager.LoadScene("UI");
    }


    public void TryLeave()
    {
        CanvasGroup group = LeavePopup.GetComponent<CanvasGroup>();
        group.alpha = 1;
        group.blocksRaycasts = true;
        group.interactable = true;
    }

    public void ClosePopup()
    {
        CanvasGroup group = LeavePopup.GetComponent<CanvasGroup>();
        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;
    }

    public void Leave()
    {
        SceneManager.LoadScene("MapScene");
    }

}
