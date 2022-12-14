using RunPG.Multi;
using System;
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
  [Header("Market place")]
  [SerializeField]
  private MarketPlace marketPlace;

  private Equipment selectedEquipment;
  CharacterProfileScript characterProfileScript;

  [Space(10)]
  [Header("Inventory Filter")]
  [SerializeField]
  private Button weaponButton;
  [SerializeField]
  private GameObject weaponBackground;

  [Space(10)]
  [SerializeField]
  private Button helmetButton;
  [SerializeField]
  private GameObject helmetBackground;

  [Space(10)]
  [SerializeField]
  private Button chestButton;
  [SerializeField]
  private GameObject chestBackground;

  [Space(10)]
  [SerializeField]
  private Button glovesButton;
  [SerializeField]
  private GameObject glovesBackground;

  [Space(10)]
  [SerializeField]
  private Button bootsButton;
  [SerializeField]
  private GameObject bootsBackground;

  [Space(10)]
  [SerializeField]
  private Button consumablesButton;
  [SerializeField]
  private GameObject consumablesBackground;

  [Space(10)]
  [SerializeField]
  private Button ressourcesButton;
  [SerializeField]
  private GameObject ressourcesBackground;
  private GameObject selectedFilterBackground;

  [Space(10)]
  [SerializeField]
  private Sprite choosedButtonSprite;
  [SerializeField]
  private Sprite selectButtonSprite;


  // Start is called before the first frame update
  void Start()
  {
    selectedFilterBackground = weaponBackground;

    characterProfileScript = CharacterProfileScript.instance;
    postOfferButton.onClick.AddListener(PostOffer);
    GetComponent<Button>().onClick.AddListener(ClosePopUp);

    weaponButton.onClick.AddListener(delegate
    {
      SelectFilter(weaponBackground);
      LoadSortedInventory(0);
    });

    helmetButton.onClick.AddListener(delegate
    {
      SelectFilter(helmetBackground);
      LoadSortedInventory(1);
    });

    chestButton.onClick.AddListener(delegate
    {
      SelectFilter(chestBackground);
      LoadSortedInventory(2);
    });

    glovesButton.onClick.AddListener(delegate
    {
      SelectFilter(glovesBackground);
      LoadSortedInventory(3);
    });

    bootsButton.onClick.AddListener(delegate
    {
      SelectFilter(bootsBackground);
      LoadSortedInventory(4);
    });

    consumablesButton.onClick.AddListener(delegate
    {
      SelectFilter(consumablesBackground);
      LoadSortedInventory(5);
    });

    ressourcesButton.onClick.AddListener(delegate
    {
      SelectFilter(ressourcesBackground);
      LoadSortedInventory(6);
    });
  }
  void SelectFilter(GameObject backgroundFilter)
  {
    var previousRectTransform = selectedFilterBackground.GetComponent<RectTransform>();
    previousRectTransform.offsetMin = new Vector2(previousRectTransform.offsetMin.x, 10);

    var acutalRectTransform = backgroundFilter.GetComponent<RectTransform>();
    acutalRectTransform.offsetMin = new Vector2(acutalRectTransform.offsetMin.x, 0);
    selectedFilterBackground = backgroundFilter;
  }

  private void LoadSortedInventory(int filterIndex)
  {
    var filteredEquipments = characterProfileScript.equipments[filterIndex];
    filteredEquipments.Sort((a, b) =>
    {
      /*if (IsEquiped(a))
          return -1;
      if (IsEquiped(b))
          return 1;*/
      if (b.level - a.level != 0)
        return b.level - a.level;
      return b.rarity - a.rarity;
    });
    LoadInventoryOfferCreator(filterIndex);
  }
  public void ClosePopUp()
  {
    Debug.Log("CLOSE");
    selectedEquipment = null;
    var canvasGroup = GetComponent<CanvasGroup>();
    canvasGroup.alpha = 0;
    canvasGroup.interactable = false;
    canvasGroup.blocksRaycasts = false;
    var marketPlaceCanvasGroup = marketPlace.GetComponent<CanvasGroup>();
    marketPlaceCanvasGroup.interactable = true;
    marketPlaceCanvasGroup.blocksRaycasts = true;
  }

  public async void PostOffer()
  {
    if (selectedEquipment == null)
    {
      errorText.text = "Pas d'�quipement s�l�ctionn�";
      return;
    }

    InventoryModel[] inventory = await Requests.GETUserInventory(PlayerProfile.id);
    foreach (var inv in inventory)
    {
      if (inv.equipmentId == selectedEquipment.id || (selectedEquipment.isItem && inv.itemId == selectedEquipment.id))
      {
        var stackSize = Int16.Parse(StackSizeInput.text);
        var newMarket = await Requests.POSTCreateItem(inv.id, Int16.Parse(goldPriceInput.text), stackSize);

        if (newMarket == null)
        {
          errorText.text = "Erreur";
          return;
        }
        marketPlace.InstantiateOfferDisplay(newMarket, selectedEquipment);
        ClosePopUp();
        CharacterProfileScript.instance.Load();
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

      if (equipment.stackSize < 1)
        continue;

      if (equipment.isItem)
      {
        var newItem = Instantiate(itemPrefab, offerCreationPopUp.transform).transform;
        newItem.GetComponent<Image>().sprite = equipment.rarity.GetSprite();
        newItem.Find("Name").GetComponent<TextMeshProUGUI>().text = equipment.name;
        newItem.Find("Image").GetComponent<Image>().sprite = equipment.GetEquipmentSprite();
        newItem.Find("Quantity").GetComponent<TextMeshProUGUI>().text = equipment.stackSize.ToString();
        Button itemButton = newItem.Find("Button").GetComponent<Button>();
        itemButton.gameObject.SetActive(true);
        itemButton.GetComponentInChildren<TextMeshProUGUI>().text = "S�lectionner";
        itemButton.onClick.AddListener(() => SelectEquipment(itemButton, equipment));
        continue;
      }
      var newEquipment = Instantiate(equipmentPrefab, offerCreationPopUp.transform).transform;

      newEquipment.Find("Image").GetComponent<Image>().sprite = equipment.GetEquipmentSprite();
      newEquipment.Find("Level").GetComponent<TextMeshProUGUI>().text = "Lv. " + equipment.level.ToString();
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
      button.GetComponentInChildren<TextMeshProUGUI>().text = "S�lectionner";
      button.onClick.AddListener(() => SelectEquipment(button, equipment)
      );

    }
  }
  public void SelectEquipment(Button button, Equipment equipment)
  {
    //Unequip
    if (selectedEquipment == null)
    {
      button.GetComponentInChildren<TextMeshProUGUI>().text = "Choisi";
      button.GetComponent<Image>().sprite = choosedButtonSprite;
      selectedEquipment = equipment;
    }
    else if (selectedEquipment == equipment)
    {
      button.GetComponentInChildren<TextMeshProUGUI>().text = "S�lectionner";
      button.GetComponent<Image>().sprite = selectButtonSprite;
      selectedEquipment = null;
    }
  }
}
