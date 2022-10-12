using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIScript : MonoBehaviour
{
    [SerializeField]
    private DungeonPortal dungeon;
    [SerializeField]
    private ForestPortal forest;

    public long id;
    private ActivityScript activity;

    void Start()
    {
        id = long.Parse(transform.name);
        if (id % 2 == 0)
        {
            dungeon.gameObject.SetActive(true);
            activity = dungeon;
        }
        else
        {
            forest.gameObject.SetActive(true);
            activity = forest;
        }
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
        return true;
    }

    public void ShowInfo()
    {
        activity.ShowInfo();
    }
}
