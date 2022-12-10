using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NewItemModel
{
    public int inventoryId;
    public int stackSize;
    public int price;

    public NewItemModel(int inventoryId, int stackSize, int price)
    {
        this.inventoryId = inventoryId;
        this.stackSize = stackSize;
        this.price = price;
    }
}