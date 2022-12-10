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
    private TextMeshProUGUI levelClass;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private Image itemImage;

    [Space(10)]
    [Header("Statistics informations")]
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
    private CanvasGroup canvasGroup;
    private CanvasGroup mainWindowCanvasGroup;

    [Space(10)]
    [Header("Buttons")]
    [SerializeField]
    private TextMeshProUGUI errorMessage;
    private MarketModel market;
    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        GetComponent<Button>().onClick.AddListener(ClosePopUp);
    }

    public void LoadPopUp(CanvasGroup mainWindowCanvasGroup, Equipment equipment, MarketModel market)
    {
        errorMessage.text = "";
        objectName.text = equipment.name;
        levelClass.text = string.Format("Nv. {0} - {1}", equipment.level, equipment.heroClass.ToString());
        description.text = equipment.description;
        backgroundImage.sprite = equipment.rarity.GetItemSprite();
        itemImage.sprite = equipment.GetEquipmentSprite();

        vitality.text = equipment.vitality.ToString();
        strength.text = equipment.strength.ToString();
        defense.text = equipment.defense.ToString();
        power.text = equipment.power.ToString();
        resistance.text = equipment.resistance.ToString();
        precision.text = equipment.precision.ToString();

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(ClosePopUp);

        this.mainWindowCanvasGroup = mainWindowCanvasGroup;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        mainWindowCanvasGroup.interactable = false;
        mainWindowCanvasGroup.blocksRaycasts = false;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(BuyEquipment);
    }
    public async void BuyEquipment()
    {
        bool res = await Requests.POSTBuyItem(market.id, PlayerProfile.id);
        if (res)
        {
            ClosePopUp();
            /* PlayerProfile.gold -= market.goldPrice;
             PlayerProfile.equipments.Add(equipment);
             PlayerProfile.Save();*/
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
