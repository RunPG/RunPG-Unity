using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfferEquipmentDisplay : OfferDisplay
{
    public Equipment equipment;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI vitalityText;
    [SerializeField]
    private TextMeshProUGUI strengthText;
    [SerializeField]
    private TextMeshProUGUI defenseText;
    [SerializeField]
    private TextMeshProUGUI powerText;
    [SerializeField]
    private TextMeshProUGUI resistanceText;
    [SerializeField]
    private TextMeshProUGUI precisionText;
    [SerializeField]
    private Image raretyImage;
    [SerializeField]
    private Image equipmentImage;
    public void SetInformations(MarketModel market, Equipment equipment)
    {
        
        base.SetInformations(market);

        this.equipment = equipment;

        raretyImage.sprite = RarityMethods.GetSprite(equipment.rarity);
        equipmentImage.sprite = EquipmentMethods.GetEquipmentSprite(equipment);
        
        nameText.text = equipment.name;
        levelText.text = equipment.level.ToString();
        vitalityText.text = equipment.vitality.ToString();
        strengthText.text = equipment.strength.ToString();
        defenseText.text = equipment.defense.ToString();
        powerText.text = equipment.power.ToString();
        resistanceText.text = equipment.resistance.ToString();
        precisionText.text = equipment.precision.ToString();
    }
}