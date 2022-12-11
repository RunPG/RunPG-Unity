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
    private CanvasGroup MyOffers;
    [SerializeField]
    private CanvasGroup AllOffers;
    
    private List<OfferDisplay> AllofferDisplays;
    private List<OfferDisplay> MyofferDisplays;
    /* [SerializeField]
     private OfferItemDisplay OfferItemDisplay;*/
    // Start is called before the first frame update
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
            if (!marketModel.isSold)
            {

                if (marketModel.equipmentId.HasValue)
                {
                   
                    if (marketModel.sellerId == PlayerProfile.id)
                    {
                        var prefab = Instantiate(OfferEquipmentDisplay, MyOffersPrefabPos);
                        var equipmentModel = await Requests.GETEquipmentById(marketModel.equipmentId.Value);
                        var equipment = new Equipment(equipmentModel);
                        prefab.SetInformations(marketModel, equipment);

                        prefab.buyButton.onClick.AddListener(() => { offerEquipmentDescription.LoadPopUp(GetComponent<CanvasGroup>(), prefab, marketModel);
                            MyofferDisplays.Remove(prefab);
                        });
                        MyofferDisplays.Add(prefab);
                    }
                    else
                    {
                        var prefab = Instantiate(OfferEquipmentDisplay, AllOffersPrefabPos);
                        var equipmentModel = await Requests.GETEquipmentById(marketModel.equipmentId.Value);
                        var equipment = new Equipment(equipmentModel);
                        prefab.SetInformations(marketModel, equipment);

                        prefab.buyButton.onClick.AddListener(() => { offerEquipmentDescription.LoadPopUp(GetComponent<CanvasGroup>(), prefab, marketModel);
                            AllofferDisplays.Remove(prefab);
                        });
                        AllofferDisplays.Add(prefab);
                    }
                }
                else
                {

                    /*var prefab = Instantiate(OfferItemDisplay, prefabPos);
                    var item = await Requests.GETEquipmentById(marketModel.itemId.Value);
                    prefab.SetInformations();*/

               
                }
            }
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