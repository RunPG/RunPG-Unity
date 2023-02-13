using RunPG.Multi;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketPlace : MonoBehaviour
{
  [Header("Prefabs")]

  [SerializeField]
  private OfferEquipmentDisplay OfferEquipmentDisplay;
  [SerializeField]
  private OfferEquipmentDisplay OfferItemDisplay;

  [Space(10)]
  [Header("All Offers")]

  [SerializeField]
  private Transform AllOffersPrefabPos;
  [SerializeField]
  private Button AllOffersButton;
  [SerializeField]
  private CanvasGroup AllOffers;

  [Space(10)]
  [Header("My Offers")]

  [SerializeField]
  private Transform MyOffersPrefabPos;
  [SerializeField]
  private Button MyOffersButton;
  [SerializeField]
  private CanvasGroup MyOffers;

  [Space(10)]
  [Header("PopUps")]
  [SerializeField]
  private OfferEquipmentDescription offerEquipmentDescription;
  [SerializeField]
  private OfferEquipmentDescription offerItemDescription;
  [SerializeField]
  private CreateOfferPopUp createOfferPopUp;


  [Space(10)]
  [Header("Buttons")]

  [SerializeField]
  private Button postOfferButton;
  private List<OfferDisplay> AllofferDisplays;
  private List<OfferDisplay> MyofferDisplays;
  async void Start()
  {
    AllofferDisplays = new List<OfferDisplay>();
    MyofferDisplays = new List<OfferDisplay>();


    AllOffersButton.onClick.AddListener(ShowAllOffers);
    MyOffersButton.onClick.AddListener(ShowMyOffers);

    postOfferButton.onClick.AddListener(OpenCreateOfferPopUp);
    //ALL OFFERS
    var allOffers = await Requests.GETallOpenItems();
    foreach (var marketModel in allOffers)
    {

      Equipment equipment;
      if (marketModel.equipmentId.HasValue)
      {

        var equipmentModel = await Requests.GETEquipmentById(marketModel.equipmentId.Value);
        equipment = new Equipment(equipmentModel, marketModel.stackSize);
      }
      else
      {
        var itemModel = await Requests.GetItemById(marketModel.itemId.Value);
        equipment = new Equipment(itemModel[0], marketModel.stackSize);
      }

      InstantiateOfferDisplay(marketModel, equipment);
    }
  }

  public void OpenCreateOfferPopUp()
  {
    var canvasGroup = createOfferPopUp.GetComponent<CanvasGroup>();
    createOfferPopUp.LoadInventoryOfferCreator(0);
    canvasGroup.alpha = 1;
    canvasGroup.interactable = true;
    canvasGroup.blocksRaycasts = true;
    var marketPlaceCanvasGroup = GetComponent<CanvasGroup>();
    marketPlaceCanvasGroup.interactable = false;
    marketPlaceCanvasGroup.blocksRaycasts = false;
  }
  public void InstantiateOfferDisplay(MarketModel market, Equipment equipment)
  {

    if (market.sellerId == PlayerProfile.id)
    {
      var prefab = Instantiate(market.equipmentId.HasValue ? OfferEquipmentDisplay : OfferItemDisplay, MyOffersPrefabPos);
      prefab.SetInformations(market, equipment);
      prefab.buyButton.onClick.AddListener(() =>
      {
        if (market.equipmentId.HasValue)
        {
          offerEquipmentDescription.LoadPopUp(GetComponent<CanvasGroup>(), prefab, market);
        }
        else
        {
          offerItemDescription.LoadPopUp(GetComponent<CanvasGroup>(), prefab, market);
        }
        
        // MyofferDisplays.Remove(prefab);
      });
      MyofferDisplays.Add(prefab);
    }
    else
    {
      var prefab = Instantiate(market.equipmentId.HasValue ? OfferEquipmentDisplay : OfferItemDisplay, AllOffersPrefabPos);
      prefab.SetInformations(market, equipment);
      prefab.buyButton.onClick.AddListener(() =>
      {
        if (market.equipmentId.HasValue)
        {
          offerEquipmentDescription.LoadPopUp(GetComponent<CanvasGroup>(), prefab, market);
        }
        else
        {
          offerItemDescription.LoadPopUp(GetComponent<CanvasGroup>(), prefab, market);
        }
      });
      AllofferDisplays.Add(prefab);
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
}