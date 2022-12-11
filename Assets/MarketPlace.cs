using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class MarketPlace : MonoBehaviour
{
    [SerializeField]
    private OfferEquipmentDisplay OfferEquipmentDisplay;
    [SerializeField]
    private OfferEquipmentDisplay OfferItemDisplay;
    [SerializeField]
    private Transform AllOffersPrefabPos;
    [SerializeField]
    private Transform MyOffersPrefabPos;
    [SerializeField]
    private OfferEquipmentDescription offerEquipmentDescription;
    [SerializeField]
    private OfferEquipmentDescription offerItemDescription;
    [SerializeField]
    private Button AllOffersButton;
    [SerializeField]
    private Button MyOffersButton;
    [SerializeField]
    private CanvasGroup MyOffers;
    [SerializeField]
    private CanvasGroup AllOffers;
    
    private List<OfferDisplay> AllofferDisplays;
    private List<OfferDisplay> MyofferDisplays;

    async void Start()
    {
        AllofferDisplays = new List<OfferDisplay>();
        MyofferDisplays = new List<OfferDisplay>();


        AllOffersButton.onClick.AddListener(ShowAllOffers);
        MyOffersButton.onClick.AddListener(ShowMyOffers);
        //ALL OFFERS
        var allOffers = await Requests.GETallOpenItems();
        foreach (var marketModel in allOffers)
        {
            Debug.Log("OFFER GOT");
            if (marketModel.equipmentId.HasValue)
            {
                Debug.Log("ISEQUIPMENT");

                var equipmentModel = await Requests.GETEquipmentById(marketModel.equipmentId.Value);
                var equipment = new Equipment(equipmentModel);
                InstantiateOfferDisplay(marketModel, equipment, OfferEquipmentDisplay);
            }
            else
            {
                Debug.Log("ITEM");
                var itemModel = await Requests.GetItemById(marketModel.itemId.Value);
                var equipment = new Equipment(itemModel[0], marketModel.stackSize);
                InstantiateOfferDisplay(marketModel, equipment, OfferItemDisplay);
            }
            
        }
    }
    public void InstantiateOfferDisplay(MarketModel market, Equipment equipment, OfferEquipmentDisplay offerDisplay)
    {
        if (market.sellerId == PlayerProfile.id)
        {
            var prefab = Instantiate(offerDisplay, MyOffersPrefabPos);
            prefab.SetInformations(market, equipment);
            prefab.buyButton.onClick.AddListener(() =>
            {
                offerEquipmentDescription.LoadPopUp(GetComponent<CanvasGroup>(), prefab, market);
                MyofferDisplays.Remove(prefab);
            });
            MyofferDisplays.Add(prefab);
        }
        else
        {
            Debug.Log("Instantiate");

            var prefab = Instantiate(offerDisplay, MyOffersPrefabPos);
            prefab.SetInformations(market, equipment);
            prefab.buyButton.onClick.AddListener(() =>
            {
                offerEquipmentDescription.LoadPopUp(GetComponent<CanvasGroup>(), prefab, market);
                MyofferDisplays.Remove(prefab);
            });
            MyofferDisplays.Add(prefab);
        } 
    }
    public void ShowAllOffers()
    {
        AllOffers.alpha = 1;
        AllOffers.interactable = true;
        AllOffers.blocksRaycasts = true;
        
        MyOffers.alpha = 0;
        MyOffers.interactable = false;
        MyOffers.blocksRaycasts = false;
    }

    public void ShowMyOffers()
    {
        AllOffers.alpha = 0;
        AllOffers.interactable = false;
        AllOffers.blocksRaycasts = false;

        MyOffers.alpha = 1;
        MyOffers.interactable = true;
        MyOffers.blocksRaycasts = true;
    }
    // Update is called once per frame
    void Update()
    {

    }
}