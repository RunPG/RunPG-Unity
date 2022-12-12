
using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class OfferDisplay : MonoBehaviour
{
    protected MarketModel market;
    [SerializeField]
    public Button buyButton;
    [SerializeField]
    protected TextMeshProUGUI goldPriceText;

    public void SetInformations(MarketModel marketModel)
    {
        market = marketModel;
        goldPriceText.text = market.goldPrice.ToString();
    }
}