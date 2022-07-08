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
        COMMON,
        RARE,
        EPIC,
        LEGENDARY,
        RELIC
    }

    public static Color getRarityColor(RarityType rarity)
    {
        return rarity switch
        {
            RarityType.COMMON => new Color(0, 255, 0),
            RarityType.RARE => new Color(0, 0, 255),
            RarityType.EPIC => new Color(102, 0, 204),
            RarityType.LEGENDARY => new Color(255, 128, 0),
            RarityType.RELIC => new Color(255, 0, 0),
            _ => new Color(0, 0, 0)
        };
    }
}