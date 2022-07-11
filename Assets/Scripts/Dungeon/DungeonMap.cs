using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonMap : MonoBehaviourPun
{ 
    public static FlexibleGridLayout flexibleGrid;
    [SerializeField]
    private Canvas LeavePopup;

    [SerializeField]
    private CanvasGroup MapCanvasGroup;
    [SerializeField]
    private CanvasGroup ResultCanvasGroup;
    [SerializeField]
    private TextMeshProUGUI ResultText;
    [SerializeField]
    private GameObject LeaveButton;
    [SerializeField]
    private GameObject StatusButton;


    private bool mapLoaded = false;

    private void Update()
    {
        if (DungeonManager.instance.map != null && !mapLoaded)
        {
            mapLoaded = true;
            flexibleGrid = GameObject.Find("Map/Scroll View/FlexibleGrid").GetComponent<FlexibleGridLayout>();


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
                //flexibleGrid.createDefeatText();
                DisableCanvasGroup(MapCanvasGroup, false);
                LeaveButton.SetActive(false);
                StatusButton.SetActive(false);
                ResultText.text = "D?faite";
                ResultText.color = Color.yellow;
                ActiveCanvasGroup(ResultCanvasGroup);
                
                StartCoroutine(flexibleGrid.AutoScroll(2f));
                victory = false;
            }
            else if (DungeonManager.instance.currentFloor == DungeonManager.instance.maxFloor)
            {
                //flexibleGrid.createVictoryText();
                DisableCanvasGroup(MapCanvasGroup, false);
                LeaveButton.SetActive(false);
                StatusButton.SetActive(false);
                ResultText.text = "Victoire";
                ResultText.color = Color.yellow;
                ActiveCanvasGroup(ResultCanvasGroup);
                
                StartCoroutine(flexibleGrid.AutoScroll(2f));
                victory = true;
            }
            flexibleGrid.displayMap(victory);
        }
    }
    public static List<List<Room>> GenerateMap(int seed)
    {
        Random.InitState(seed);
        int dungeonSize = Random.Range(4, 5);
        List<List<Room>> map = new List<List<Room>>(dungeonSize);
        map.Add(new List<Room>() { new StartRoom() });
        for (int i = 1; i < dungeonSize; i++)
        {
            int rowSize = Random.Range(1, 4);
            List<Room> row = new List<Room>(rowSize);
            for (int j = 0; j < rowSize; j++)
            {
                var r = Random.Range(0, 10);
                switch (r)
                {
                    case < 2:
                        row.Add(new HealRoom());
                        break;

                    case < 3:
                        row.Add(new BonusRoom());
                        break;

                    default:
                        FightRoom room = new FightRoom(DungeonManager.instance.generateFightEnemies(i));
                        row.Add(room);
                        break;
                }
            }
            map.Add(row);
        }
        map.Add(new List<Room>() { new BossRoom(DungeonManager.instance.generateBossEnemies()) });
        return map;
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
        if (PhotonNetwork.IsMasterClient)
        {
            DungeonManager.instance.LeaveDungeon();
        }
        else
        {
            DungeonManager.instance.Leave();
        }
    }

    public static void RefreshMap()
    {
        foreach (Transform child in flexibleGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        flexibleGrid.displayMap(null);
    }

    public static void DisableCanvasGroup(CanvasGroup canvasGroup, bool alpha = true)
    {
        canvasGroup.alpha = alpha ? 0 : 1;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public static void ActiveCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
}
