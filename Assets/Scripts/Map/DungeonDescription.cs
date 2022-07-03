using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDescription : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup descriptionCanvas;

    public GameObject dungeon;

    public void Hide()
    {
        dungeon = null;
        descriptionCanvas.alpha = 0;
        descriptionCanvas.interactable = false;
        descriptionCanvas.blocksRaycasts = false;
    }

    public void Enter()
    {
        dungeon.GetComponent<DungeonPortal>().EnterDungeon();
    }
}
