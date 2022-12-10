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
    private Transform AllOffersPrefabPos;
    [SerializeField]
    private Transform MyOffersPrefabPos;
    [SerializeField]
    private OfferEquipmentDescription offerEquipmentDescription;
    [SerializeField]
    private Button AllOffersButton;
    [SerializeField]
    private Button MyOffersButton;
    [SerializeField]
    private GameObject MyOffers;
    [SerializeField]
    private GameObject AllOffers;
    
    private List<OfferDisplay> AllofferDisplays;
    private List<OfferDisplay> MyofferDisplays;
    /* [SerializeField]
     private OfferItemDisplay OfferItemDisplay;*/
    // Start is called before the first frame update
    async void Start()
    {

        //ALL OFFERS
        var allOffers = await Requests.GETallOpenItems();
        foreach (var marketModel in allOffers)
        {
            Debug.Log(marketModel.id);
            if (!marketModel.isSold)
            {
                if (marketModel.equipmentId.HasValue)
                {
                    var prefab = Instantiate(OfferEquipmentDisplay, AllOffersPrefabPos);
                    var equipmentModel = await Requests.GETEquipmentById(marketModel.equipmentId.Value);
                    var equipment = new Equipment(equipmentModel);
                    prefab.SetInformations(marketModel, equipment);
                    
                    prefab.buyButton.onClick.AddListener(() => { offerEquipmentDescription.LoadPopUp(GetComponent<CanvasGroup>(), prefab, marketModel); });
                    AllofferDisplays.Add(prefab);
                }
                else
                {
                    
                    /*var prefab = Instantiate(OfferItemDisplay, prefabPos);
                    var item = await Requests.GETEquipmentById(marketModel.itemId.Value);
                    prefab.SetInformations();*/
                     
                }
            }
        }
        //MY OFFERS
       /* var marketModels = await Requests.GetAll();
        foreach (var marketModel in marketModels)
        {
            Debug.Log(marketModel.id);
            if (!marketModel.isSold)
            {
                if (marketModel.equipmentId.HasValue)
                {
                    var prefab = Instantiate(OfferEquipmentDisplay, AllOffersPrefabPos);
                    var equipmentModel = await Requests.GETEquipmentById(marketModel.equipmentId.Value);
                    var equipment = new Equipment(equipmentModel);
                    prefab.SetInformations(marketModel, equipment);

                    prefab.buyButton.onClick.AddListener(() => { offerEquipmentDescription.LoadPopUp(GetComponent<CanvasGroup>(), prefab, marketModel); });
                    AllofferDisplays.Add(prefab);
                }
                else
                {
                    /*var prefab = Instantiate(OfferItemDisplay, prefabPos);
                    var item = await Requests.GETEquipmentById(marketModel.itemId.Value);
                    prefab.SetInformations();*/

           /*     }
            }
        }*/

    }

    // Update is called once per frame
    void Update()
    {

    }
}