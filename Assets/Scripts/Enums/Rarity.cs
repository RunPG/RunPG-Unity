using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum Rarity
{
    COMMON,
    RARE,
    EPIC,
    LEGENDARY,
    RELIC
}

static class RarityMethods
{

    public static Color GetColor(this Rarity rarity)
    {
        return rarity switch
        {
            Rarity.COMMON => new Color(0, 255, 0),
            Rarity.RARE => new Color(0, 0, 255),
            Rarity.EPIC => new Color(102, 0, 204),
            Rarity.LEGENDARY => new Color(255, 128, 0),
            Rarity.RELIC => new Color(255, 0, 0),
            _ => new Color(0, 0, 0)
        };
    }

    public static string GetName(this Rarity rarity)
    {
        return rarity switch
        {
            Rarity.COMMON => "Commun",
            Rarity.RARE => "Rare",
            Rarity.EPIC => "Epic",
            Rarity.LEGENDARY => "Légendaire",
            Rarity.RELIC => "Relique",
            _ => ""
        };
    }
}