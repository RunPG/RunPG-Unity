using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class OfferEquipmentDescription : MonoBehaviour
{

    [Header("Item Informations")]
    [SerializeField]
    private TextMeshProUGUI description;
    [SerializeField]
    private TextMeshProUGUI objectName;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private Image itemImage;

    [Space(10)]
    [Header("Statistics informations")]
    [SerializeField]
    private TextMeshProUGUI levelClass;
    [SerializeField]
    private TextMeshProUGUI vitality;
    [SerializeField]
    private TextMeshProUGUI strength;
    [SerializeField]
    private TextMeshProUGUI defense;
    [SerializeField]
    private TextMeshProUGUI power;
    [SerializeField]
    private TextMeshProUGUI resistance;
    [SerializeField]
    private TextMeshProUGUI precision;

    [Space(10)]
    [Header("Buttons")]
    [SerializeField]
    private Button buyButton;
    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private Button deleteButton;
    private CanvasGroup canvasGroup;
    private CanvasGroup mainWindowCanvasGroup;

    [Space(10)]
    [Header("Error")]
    [SerializeField]
    private TextMeshProUGUI errorMessage;
    private MarketModel market;
    private OfferEquipmentDisplay offerEquipmentDisplay;
    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        GetComponent<Button>().onClick.AddListener(ClosePopUp);
    }

    public void LoadPopUp(CanvasGroup mainWindowCanvasGroup, OfferEquipmentDisplay offerEquipmentDisplay, MarketModel market)
    {
        this.market = market;
        this.offerEquipmentDisplay = offerEquipmentDisplay;
        errorMessage.text = "";
        objectName.text = offerEquipmentDisplay.equipment.name;
        levelClass.text = string.Format("Nv. {0} - {1}", offerEquipmentDisplay.equipment.level, offerEquipmentDisplay.equipment.heroClass.ToString());
        description.text = offerEquipmentDisplay.equipment.description;
        backgroundImage.sprite = offerEquipmentDisplay.equipment.rarity.GetItemSprite();
        itemImage.sprite = offerEquipmentDisplay.equipment.GetEquipmentSprite();
        
        if (!offerEquipmentDisplay.equipment.isItem)
        {
            vitality.text = offerEquipmentDisplay.equipment.vitality.ToString();
            strength.text = offerEquipmentDisplay.equipment.strength.ToString();
            defense.text = offerEquipmentDisplay.equipment.defense.ToString();
            power.text = offerEquipmentDisplay.equipment.power.ToString();
            resistance.text = offerEquipmentDisplay.equipment.resistance.ToString();
            precision.text = offerEquipmentDisplay.equipment.precision.ToString();
        }
        
        this.mainWindowCanvasGroup = mainWindowCanvasGroup;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        mainWindowCanvasGroup.interactable = false;
        mainWindowCanvasGroup.blocksRaycasts = false;

        buyButton.onClick.RemoveAllListeners();
        deleteButton.onClick.RemoveAllListeners();       
        closeButton.onClick.RemoveAllListeners();
        
        closeButton.onClick.AddListener(ClosePopUp);



        if (market.sellerId == PlayerProfile.id)
        {
            deleteButton.onClick.AddListener(DeleteMyOffer);
            deleteButton.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            deleteButton.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(true);
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = market.goldPrice.ToString();
            buyButton.onClick.AddListener(BuyEquipment);
        }
    }
    public async void DeleteMyOffer()
    {
        Debug.Log("DELETE OFFER");
        await Requests.DELETEItem(market.id);
        offerEquipmentDisplay.gameObject.Destroy();
        ClosePopUp();
    }
    public async void BuyEquipment()
    {
        bool res = await Requests.POSTBuyItem(market.id, PlayerProfile.id);
        if (res)
        {            
            ClosePopUp();
            Destroy(offerEquipmentDisplay.gameObject);
        }
        else
        {     
            errorMessage.text = "Achat impossible !";
            //error msg;
        }
    }
    public void ClosePopUp()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        mainWindowCanvasGroup.interactable = true;
        mainWindowCanvasGroup.blocksRaycasts = true;
    }

}
