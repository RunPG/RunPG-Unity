
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class MarketModel
{
    public int id;
    public int sellerId;
    public int? equipmentId;
    public int? itemId;
    public int stackSize;
    public int goldPrice;
    public bool isSold;

    public MarketModel(int id, int sellerId, int? equipmentId, int? itemId, int stackSize, int goldPrice, bool isSold)
    {
        this.id = id;
        this.sellerId = sellerId;
        this.equipmentId = equipmentId;
        this.itemId = itemId;
        this.stackSize = stackSize;
        this.goldPrice = goldPrice;
        this.isSold = isSold;
    }
}