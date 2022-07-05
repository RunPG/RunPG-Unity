using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonDescription : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup descriptionCanvas;

    [SerializeField]
    private TextMeshProUGUI distanceWarning;

    public GameObject dungeon;

    public void Hide()
    {
        dungeon = null;
        descriptionCanvas.alpha = 0;
        descriptionCanvas.interactable = false;
        descriptionCanvas.blocksRaycasts = false;
        distanceWarning.alpha = 0;
    }

    public void Enter()
    {
        if (!dungeon.GetComponent<DungeonPortal>().EnterDungeon())
        {
            distanceWarning.alpha = 1;
        }
    }
}
