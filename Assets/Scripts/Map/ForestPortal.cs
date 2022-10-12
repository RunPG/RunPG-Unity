using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForestPortal : ActivityScript
{
    [SerializeField]
    private POIScript poi;

    private PortalDescription description = null;

    public override int range => 50;

    public override int cooldown => 0;

    public override void Enter()
    {
        if (IsInRange())
        {
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
}
