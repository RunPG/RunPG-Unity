using RunPG.Multi;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateOfferPopUp : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject equipmentPrefab;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private GameObject offerCreationPopUp;
    
    [Space(10)]
    [Header("Inputs")]
    [SerializeField]
    private TMP_InputField goldPriceInput;
    [SerializeField]
    private TMP_InputField StackSizeInput;
    [SerializeField]
    private Button postOfferButton;
    
    [Space(10)]
    [Header("Error")]
    [SerializeField]
    private TextMeshProUGUI errorText;
    [Space(10)]
    [Header("Canvas Group")]
    [SerializeField]
    private CanvasGroup marketPlaceCanvasGroup;
    private MarketPlace marketPlace;

    private Equipment selectedEquipment;
    CharacterProfileScript characterProfileScript;
    
    // Start is called before the first frame update
    void Start()
    {
        characterProfileScript = CharacterProfileScript.instance;
        postOfferButton.onClick.AddListener(PostOffer);
        GetComponent<Button>().onClick.AddListener(ClosePopUp);
    }
    public void ClosePopUp()
    {
        Debug.Log("CLOSE");
        selectedEquipment = null;
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        marketPlaceCanvasGroup.interactable = true;
        marketPlaceCanvasGroup.blocksRaycasts = true;
    }

    public async void PostOffer()
    {
        if (selectedEquipment == null)
        {
            errorText.text = "Pas d'équipement séléctionné";
            return;
        }

        InventoryModel[] inventory = await Requests.GETUserInventory(PlayerProfile.id);
        foreach (var inv in inventory)
        {
            if (inv.equipmentId == selectedEquipment.id)
            {              
                var newMarket = await Requests.POSTCreateItem(inv.id, Int16.Parse(goldPriceInput.text), Int16.Parse(StackSizeInput.text));
                marketPlace.InstantiateOfferDisplay(newMarket, selectedEquipment);
                ClosePopUp();
                return;
            }
        }
        Debug.Log("ERROR GET INVENTORY ID");    
    }
    public void LoadInventoryOfferCreator(int filterIndex)
    {
        Debug.Log("LOAD INVENTORY");
        foreach (Transform child in offerCreationPopUp.transform)
        {
            Destroy(child.gameObject);
        }

        var filterList = characterProfileScript.equipments[filterIndex];

        if (filterList.Count == 0)
            offerCreationPopUp.GetComponent<VerticalLayoutGroup>().padding.bottom = 0;
        else
            offerCreationPopUp.GetComponent<VerticalLayoutGroup>().padding.bottom = 8;

        for (int equipmentIndex = 0; equipmentIndex < filterList.Count; equipmentIndex++)
        {           
            var equipment = filterList[equipmentIndex];

            if (equipment.isItem)
            {
                var newItem = Instantiate(itemPrefab, offerCreationPopUp.transform).transform;
                newItem.GetComponent<Image>().sprite = equipment.rarity.GetSprite();
                newItem.Find("Name").GetComponent<TextMeshProUGUI>().text = equipment.name;
                newItem.Find("Image").GetComponent<Image>().sprite = equipment.GetEquipmentSprite();
                newItem.Find("Quantity").GetComponent<TextMeshProUGUI>().text = equipment.stackSize.ToString();
                continue;
            }
            var newEquipment = Instantiate(equipmentPrefab, offerCreationPopUp.transform).transform;

            newEquipment.Find("Image").GetComponent<Image>().sprite = equipment.GetEquipmentSprite();
            newEquipment.Find("Name").GetComponent<TextMeshProUGUI>().text = equipment.name;

            newEquipment.GetComponent<Image>().sprite = equipment.rarity.GetSprite();
            var equipedItem = characterProfileScript.GetEquiped(equipment.type);

            var vitalityText = newEquipment.Find("Stats/Vitality/Value").GetComponent<TextMeshProUGUI>();
            vitalityText.text = equipment.vitality.ToString();
            vitalityText.color = characterProfileScript.GetStatisticColor(equipedItem.vitality, equipment.vitality);

            var strengthText = newEquipment.Find("Stats/Strength/Value").GetComponent<TextMeshProUGUI>();
            strengthText.text = equipment.strength.ToString();
            strengthText.color = characterProfileScript.GetStatisticColor(equipedItem.strength, equipment.strength);

            var defenseText = newEquipment.Find("Stats/Defense/Value").GetComponent<TextMeshProUGUI>();
            defenseText.text = equipment.defense.ToString();
            defenseText.color = characterProfileScript.GetStatisticColor(equipedItem.defense, equipment.defense);

            var resistanceText = newEquipment.Find("Stats/Resistance/Value").GetComponent<TextMeshProUGUI>();
            resistanceText.text = equipment.resistance.ToString();
            resistanceText.color = characterProfileScript.GetStatisticColor(equipedItem.resistance, equipment.resistance);

            var powerText = newEquipment.Find("Stats/Power/Value").GetComponent<TextMeshProUGUI>();
            powerText.text = equipment.power.ToString();
            powerText.color = characterProfileScript.GetStatisticColor(equipedItem.power, equipment.power);

            var precisionText = newEquipment.Find("Stats/Precision/Value").GetComponent<TextMeshProUGUI>();
            precisionText.text = equipment.precision.ToString();
            precisionText.color = characterProfileScript.GetStatisticColor(equipedItem.precision, equipment.precision);

            var isEquiped = equipedItem.id == equipment.id;

            Button button = newEquipment.Find("Button").GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Sélectionner";
            button.onClick.AddListener(() => SelectEquipment(button,equipment)
            );       
            
        }
    }
    public void SelectEquipment(Button button,Equipment equipment)
    {
        //Unequip
        if (selectedEquipment == null)
        {
            Debug.Log("CHOISI");
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Choisi";
            selectedEquipment = equipment;
        }
        else if (selectedEquipment == equipment)
        {
            Debug.Log("DESEQUIPER");
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Sélectionner";
            selectedEquipment = null;
        }
    }
}
