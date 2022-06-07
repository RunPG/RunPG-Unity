using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonMap : MonoBehaviour
{
    [SerializeField]
    private Button[] buttons;
    [SerializeField]
    private Canvas ResultScreen;

    private void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i != DungeonManager.instance.currentFloor)
            {
                buttons[i].interactable = false;
            }
        }

        bool alive = false;
        foreach (var character in DungeonManager.instance.characters)
        {
            if (character.currentHP > 0)
                alive = true;
        }
        if (!alive)
        {
            ResultScreen.GetComponent<CanvasGroup>().alpha = 1;
            ResultScreen.GetComponentInChildren<TextMeshProUGUI>().text = "Défaite";
            ResultScreen.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        }
        else if (DungeonManager.instance.currentFloor == DungeonManager.maxFloor)
        {
            ResultScreen.GetComponent<CanvasGroup>().alpha = 1;
            ResultScreen.GetComponentInChildren<TextMeshProUGUI>().text = "Victoire";
            ResultScreen.GetComponentInChildren<TextMeshProUGUI>().color = Color.yellow;
        }
    }

    public void LoadLevel(int level)
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
                break;
        }

        DungeonManager.instance.SetMonsters(enemies);

        SceneManager.LoadScene("UI");
    }
}
