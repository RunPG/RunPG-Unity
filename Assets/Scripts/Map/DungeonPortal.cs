using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonPortal : ActivityScript
{
    [SerializeField]
    private POIScript poi;
    [SerializeField]
    private GameObject availableDungeon;
    [SerializeField]
    private GameObject unavailableDungeon;

    private PortalDescription description = null;

    public override int range => 50;

    public override int cooldown => 5;

    public override void Enter()
    {
        if (IsInRange())
        {
            LobbyManager.instance.FindDungeonLobbies(poi.id);
        }
    }

    public override bool IsInRange()
    {
        GameObject player = GameObject.Find("LocationBasedGame/Character");
        return true;// Vector3.Distance(player.transform.position, transform.position) < range;
    }

    public override void ShowInfo()
    {
        if (!description)
            description = GameObject.Find("UI/PortalDescription").GetComponent<PortalDescription>();
        description.Show(poi, "Donjon");
    }

    public override void SetAvailable(bool state)
    {
        availableDungeon.SetActive(state);
        unavailableDungeon.SetActive(!state);
    }
}
