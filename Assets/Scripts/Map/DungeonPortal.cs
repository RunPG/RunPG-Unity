using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonPortal : MonoBehaviour
{
    public void EnterDungeon()
    {
        SceneManager.LoadScene("DungeonScene");
    }

    public void ShowInfo()
    {
        GameObject description = GameObject.Find("DungeonDescription");
        description.GetComponent<CanvasGroup>().interactable = true;
        description.GetComponent<CanvasGroup>().blocksRaycasts = true;
        description.GetComponent<CanvasGroup>().alpha = 1;
        description.GetComponent<DungeonDescription>().dungeon = gameObject;
        description.transform.Find("Background/Title").GetComponent<TextMeshProUGUI>().text = transform.name;
    }
}
