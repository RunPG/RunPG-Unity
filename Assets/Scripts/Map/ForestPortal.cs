using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForestPortal : ActivityScript
{
    [SerializeField]
    private POIScript poi;
    [SerializeField]
    private GameObject availableForest;
    [SerializeField]
    private GameObject unavailableForest;

    private PortalDescription description = null;

    public override int range => 50;

    public override int cooldown => 5;

    public override async void Enter()
    {
        if (IsInRange())
        {
            await Requests.POSTActivity(PlayerProfile.id, poi.id);
            SceneManager.LoadScene("TimbermanScene");
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
        description.Show(poi, "Forêt");
    }

    public override void SetAvailable(bool state)
    {
        availableForest.SetActive(state);
        unavailableForest.SetActive(!state);
    }
}
