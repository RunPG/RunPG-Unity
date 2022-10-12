using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonPortal : ActivityScript
{
    [SerializeField]
    private POIScript poi;

    private PortalDescription description = null;

    public override int range => 50;

    public override int cooldown => 30;

    public override void Enter()
    {
        if (IsInRange())
        {
            LobbyManager.instance.FindDungeonLobbies();
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
}
