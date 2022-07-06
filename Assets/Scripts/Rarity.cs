using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Rarity
{
    public enum RarityType
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Relic
    }

    public static Color getRarityColor(RarityType rarity)
    {
        return rarity switch
        {
            RarityType.Common => new Color(0, 255, 0),
            RarityType.Rare => new Color(0, 0, 255),
            RarityType.Epic => new Color(102, 0, 204),
            RarityType.Legendary => new Color(255, 128, 0),
            RarityType.Relic => new Color(255, 0, 0),
            _ => new Color(0, 0, 0)
        };
    }
}