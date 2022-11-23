using RunPG.Multi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class POIScript : MonoBehaviour
{
    [SerializeField]
    private DungeonPortal dungeon;
    [SerializeField]
    private ForestPortal forest;
    [SerializeField]
    private BushPortal bushes;
    [SerializeField]
    private MinesPortal mines;

    public long id;
    private ActivityScript activity;

    private DateTime lastAccess;

    private bool isAvailable;

    async void Start()
    {
        id = long.Parse(transform.name);
        if (id % 4 == 0)
        {
            dungeon.gameObject.SetActive(true);
            activity = dungeon;
        }
        else if (id % 4 == 1)
        {
            forest.gameObject.SetActive(true);
            activity = forest;
        }
        else if (id % 4 == 2)
        {
            bushes.gameObject.SetActive(true);
            activity = bushes;
        }
        else
        {
            mines.gameObject.SetActive(true);
            activity = mines;
        }

        ActivityModel availability = await Requests.GetActivityAvailability(PlayerProfile.id, id);
        lastAccess = new DateTime(1970, 1, 1, 0, 0, 0) + TimeSpan.FromMilliseconds(availability.lastAccess);
        activity.SetAvailable(IsPOIAvailable());
    }

    public void UsePOI()
    {
        activity.Enter();
    }

    public bool IsInRange()
    {
        return activity.IsInRange();
    }

    public bool IsPOIAvailable()
    {
        isAvailable = DateTime.UtcNow >= lastAccess + TimeSpan.FromMinutes(activity.cooldown);
        return isAvailable;
    }

    public void ShowInfo()
    {
        activity.ShowInfo();
    }

    private void Update()
    {
        if (!isAvailable && IsPOIAvailable())
        {
            activity.SetAvailable(true);
        }
    }
}
