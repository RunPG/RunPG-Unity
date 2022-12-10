using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinesPortal : ActivityScript
{
    [SerializeField]
    private POIScript poi;
    [SerializeField]
    private GameObject availableMines;
    [SerializeField]
    private GameObject unavailableMines;

    private PortalDescription description = null;

    public override int range => 50;

    public override int cooldown => 5;

    public override async void Enter()
    {
        if (IsInRange())
        {
            await Requests.POSTActivity(PlayerProfile.id, poi.id);
            SceneManager.LoadScene("DemineurScene");
        }
    }

    public override bool IsInRange()
    {
        return true;
    }

    public override void ShowInfo()
    {
        if (!description)
            description = GameObject.Find("UI/PortalDescription").GetComponent<PortalDescription>();
        description.Show(poi, "Mines");
    }

    public override void SetAvailable(bool state)
    {
        availableMines.SetActive(state);
        unavailableMines.SetActive(!state);
    }
}
