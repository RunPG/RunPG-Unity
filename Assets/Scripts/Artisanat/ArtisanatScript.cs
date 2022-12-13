using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Linq;
using RunPG.Multi;
using UnityEngine.UI;
using Mapbox.Json;

public class ArtisanatScript : MonoBehaviour
{
  [SerializeField]
  private Transform craftsLayout;
  [SerializeField]
  private GameObject craftPrefab;
  [SerializeField]
  private GameObject materialPrefab;
  [SerializeField]
  private TMP_InputField textInput;

  [Space(10)]
  [Header("Other Canvas")]
  [SerializeField]
  private CanvasGroup confirmationCanvasGroup;
  [SerializeField]
  private CanvasGroup craftedObjectCanvasGroup;
  [SerializeField]
  private CanvasGroup craftedItemCanvasGroup;

  private string filter;

  private List<InventoryModel> inventory;

  private List<CraftModel> crafts;
  private List<CraftModel> filteredCrafts;

  void Start()
  {
    textInput.onValueChanged.AddListener(delegate
    {
      FilterList(textInput.text);
    });
    filter = textInput.text;
    Load();
  }

  void LoadCrafts()
  {
    crafts = JsonConvert.DeserializeObject<CraftModel[]>(Resources.Load<TextAsset>("Artisanat/Crafts").text).ToList();
  }

  async Task LoadInventory()
  {
    inventory = (await Requests.GETUserInventory(PlayerProfile.id)).ToList();
  }

  void ClearCraftsList()
  {
    foreach (Transform child in craftsLayout)
    {
      Destroy(child.gameObject);
    }
  }

  void AddCrafts()
  {
    foreach (var craft in filteredCrafts)
    {
      var craftObject = Instantiate(craftPrefab, craftsLayout).transform;
      if (craft.equipementBase != null)
      {
        craftObject.GetComponent<Image>().sprite = craft.equipementBase.rarity.GetSprite();
        craftObject.Find("CraftIcon").GetComponent<Image>().sprite = craft.equipementBase.GetEquipmentSprite();
        craftObject.Find("Name").GetComponent<TextMeshProUGUI>().text = craft.equipementBase.name;
        craftObject.Find("Class").GetComponent<TextMeshProUGUI>().text = craft.equipementBase.heroClass.GetName();
      }
      else
      {
        craftObject.GetComponent<Image>().sprite = craft.item.GetRarity().GetSprite();
        craftObject.Find("CraftIcon").GetComponent<Image>().sprite = craft.item.GetSprite();
        craftObject.Find("Name").GetComponent<TextMeshProUGUI>().text = craft.item.name;
        craftObject.Find("Class").GetComponent<TextMeshProUGUI>().text = "";
      }
      var materialsTransform = craftObject.Find("Materials").transform;
      bool canCraft = true;
      foreach (var material in craft.materials)
      {
        var foundMaterial = inventory.Find(x => x.itemId == material.id);
        bool hasMaterials = foundMaterial != null && foundMaterial.stackSize >= material.quantity;
        var materialObject = Instantiate(materialPrefab, materialsTransform).transform;
        materialObject.Find("Rarity").GetComponent<Image>().sprite = material.rarity.GetItemSprite();
        materialObject.Find("Rarity/Material").GetComponent<Image>().sprite = material.GetSprite();
        materialObject.Find("Quantity").GetComponent<TextMeshProUGUI>().text = material.quantity.ToString();
        if (!hasMaterials)
        {
          materialObject.Find("Quantity").GetComponent<TextMeshProUGUI>().color = Color.red;
          canCraft = false;
        }
      }
      var craftButton = craftObject.Find("Craft").GetComponent<Button>();
      craftButton.onClick.RemoveAllListeners();
      craftButton.onClick.AddListener(() => Craft(craft));
      craftButton.interactable = canCraft;
    }
  }

  bool hasMaterials(CraftModel craft, int quantity = 1)
  {
    foreach (var material in craft.materials)
    {
      var foundMaterial = inventory.Find(x => x.itemId == material.id);
      if (foundMaterial == null || foundMaterial.stackSize < (material.quantity * quantity))
      {
        return false;
      }
    }
    return true;
  }

  void Craft(CraftModel craft)
  {
    var canvasGroup = GetComponent<CanvasGroup>();
    canvasGroup.interactable = false;
    canvasGroup.blocksRaycasts = false;

    confirmationCanvasGroup.alpha = 1;
    confirmationCanvasGroup.interactable = true;
    confirmationCanvasGroup.blocksRaycasts = true;

    var confirmationTransform = confirmationCanvasGroup.transform.Find("Background");

    confirmationTransform.Find("Name").GetComponent<TextMeshProUGUI>().text = craft.equipementBase != null ? craft.equipementBase.name : craft.item.name;
    confirmationTransform.Find("Rarity").GetComponent<Image>().sprite = craft.equipementBase != null ? craft.equipementBase.rarity.GetItemSprite() : craft.item.GetRarity().GetItemSprite();
    confirmationTransform.Find("Rarity/Item").GetComponent<Image>().sprite = craft.equipementBase != null ? craft.equipementBase.GetEquipmentSprite() : craft.item.GetSprite();

    var materialGrid = confirmationTransform.Find("Materials").transform;

    void LoadMaterials(int quantity)
    {
      foreach (Transform child in materialGrid)
      {
        Destroy(child.gameObject);
      }
      foreach (CraftItemModel material in craft.materials)
      {
        var materialObject = Instantiate(materialPrefab, materialGrid).transform;
        materialObject.Find("Rarity").GetComponent<Image>().sprite = material.rarity.GetItemSprite();
        materialObject.Find("Rarity/Material").GetComponent<Image>().sprite = material.GetSprite();
        materialObject.Find("Quantity").GetComponent<TextMeshProUGUI>().text = (material.quantity * quantity).ToString();
      }
    }

    var itemQuantity = 1;
    var quantityObject = confirmationTransform.Find("Quantity");
    quantityObject.gameObject.SetActive(craft.equipementBase == null);
    if (craft.equipementBase == null)
    {
      var addButton = quantityObject.Find("Add").GetComponent<Button>();
      var removeButton = quantityObject.Find("Remove").GetComponent<Button>();

      void SetQuantity(int quantity)
      {
        itemQuantity = quantity;
        quantityObject.Find("Value").GetComponent<TextMeshProUGUI>().text = quantity.ToString();
        LoadMaterials(quantity);
        addButton.interactable = hasMaterials(craft, quantity + 1);
        removeButton.interactable = quantity > 1;
      }
      SetQuantity(1);
      addButton.onClick.RemoveAllListeners();
      addButton.onClick.AddListener(() => SetQuantity(itemQuantity + 1));

      removeButton.onClick.RemoveAllListeners();
      removeButton.onClick.AddListener(() => SetQuantity(itemQuantity - 1));
    }
    else
      LoadMaterials(1);

    var confirmButton = confirmationTransform.Find("Confirm").GetComponent<Button>();
    confirmButton.onClick.RemoveAllListeners();
    confirmButton.onClick.AddListener(async () =>
    {
      await DeduceMaterials(craft, itemQuantity);
      confirmationCanvasGroup.alpha = 0;
      confirmationCanvasGroup.interactable = false;
      confirmationCanvasGroup.blocksRaycasts = false;
      if (craft.equipementBase != null)
      {
        Equipment equipment = await CraftNewEquipement(craft.equipementBase);
        showCraftedObject(equipment);
        return;
      }
      else
      {
        await CraftItem(craft, itemQuantity);
        showCraftedObject(craft.item, itemQuantity);
      }
    });
  }

  async Task<Equipment> CraftNewEquipement(EquipmentBaseModel equipmentBase)
  {
    StatisticsModel statistics = new StatisticsModel(0, 5, 0, 1, 2, 0, 2, 0);
    if (PlayerProfile.characterInfo.heroClass == HeroClass.MAGE)
      statistics = new StatisticsModel(0, 5, 0, 0, 0, 4, 0, 1);
    NewEquipementModel equipment = new NewEquipementModel(equipmentBase.id, statistics);
    var craftedEquipment = await Requests.POSTInventoryEquipement(PlayerProfile.id, equipment);
    var equipmentModel = await Requests.GETEquipmentById(craftedEquipment.equipmentId.Value);
    var newEquipment = new Equipment(equipmentModel);

    return newEquipment;
  }

  async Task CraftItem(CraftModel craft, int quantity)
  {
    await Requests.POSTInventoryItem(PlayerProfile.id, new PostItemModel(craft.item.id, quantity));
  }

  async Task DeduceMaterials(CraftModel craft, int quantity)
  {
    foreach (var material in craft.materials)
    {
      inventory.Find(x => x.itemId == material.id).stackSize -= material.quantity * quantity;
      await Requests.DELETEInventoryItem(PlayerProfile.id, new PostItemModel(material.id, material.quantity * quantity));
    }
    ReloadArtisanatList();
  }

  void showCraftedObject(Equipment equipment)
  {
    craftedObjectCanvasGroup.alpha = 1;
    craftedObjectCanvasGroup.interactable = true;
    craftedObjectCanvasGroup.blocksRaycasts = true;
    Transform craftedObject = craftedObjectCanvasGroup.transform.Find("Background/Body");

    craftedObject.Find("Item/Name").GetComponent<TextMeshProUGUI>().text = equipment.name;
    craftedObject.Find("Item/LevelClass").GetComponent<TextMeshProUGUI>().text = string.Format("Nv. {0} - {1}", equipment.level, equipment.heroClass);
    craftedObject.Find("Item/Description").GetComponent<TextMeshProUGUI>().text = equipment.description;
    craftedObject.Find("Item/Rarity").GetComponent<Image>().sprite = equipment.rarity.GetItemSprite();
    craftedObject.Find("Item/Rarity/Item").GetComponent<Image>().sprite = equipment.GetEquipmentSprite();

    craftedObject.Find("Statistics/Vitality/Value").GetComponent<TextMeshProUGUI>().text = equipment.vitality.ToString();
    craftedObject.Find("Statistics/Strength/Value").GetComponent<TextMeshProUGUI>().text = equipment.strength.ToString();
    craftedObject.Find("Statistics/Defense/Value").GetComponent<TextMeshProUGUI>().text = equipment.defense.ToString();
    craftedObject.Find("Statistics/Power/Value").GetComponent<TextMeshProUGUI>().text = equipment.power.ToString();
    craftedObject.Find("Statistics/Resistance/Value").GetComponent<TextMeshProUGUI>().text = equipment.resistance.ToString();
    craftedObject.Find("Statistics/Precision/Value").GetComponent<TextMeshProUGUI>().text = equipment.precision.ToString();

    var closeButton = craftedObject.Find("Buttons/Close").GetComponent<Button>();
    closeButton.onClick.RemoveAllListeners();
    closeButton.onClick.AddListener(() =>
    {
      CharacterProfileScript.instance.Load();
      craftedObjectCanvasGroup.alpha = 0;
      craftedObjectCanvasGroup.interactable = false;
      craftedObjectCanvasGroup.blocksRaycasts = false;
      var canvasGroup = GetComponent<CanvasGroup>();
      canvasGroup.interactable = true;
      canvasGroup.blocksRaycasts = true;
    });

    var equipButton = craftedObject.Find("Buttons/Equip").GetComponent<Button>();
    equipButton.interactable = equipment.heroClass == PlayerProfile.characterInfo.heroClass;
    equipButton.onClick.RemoveAllListeners();
    equipButton.onClick.AddListener(() =>
    {
      CharacterProfileScript.instance.Equip(equipment);
      CharacterProfileScript.instance.Load();
      craftedObjectCanvasGroup.alpha = 0;
      craftedObjectCanvasGroup.interactable = false;
      craftedObjectCanvasGroup.blocksRaycasts = false;
      var canvasGroup = GetComponent<CanvasGroup>();
      canvasGroup.interactable = true;
      canvasGroup.blocksRaycasts = true;
    });
  }

  void showCraftedObject(ItemModel item, int quantity)
  {
    craftedItemCanvasGroup.alpha = 1;
    craftedItemCanvasGroup.interactable = true;
    craftedItemCanvasGroup.blocksRaycasts = true;
    Transform craftedObject = craftedItemCanvasGroup.transform.Find("Background/Body");

    craftedObject.Find("Item/Name").GetComponent<TextMeshProUGUI>().text = item.name;
    craftedObject.Find("Item/Description").GetComponent<TextMeshProUGUI>().text = item.description;
    craftedObject.Find("Item/Rarity").GetComponent<Image>().sprite = item.GetRarity().GetItemSprite();
    craftedObject.Find("Item/Rarity/Item").GetComponent<Image>().sprite = item.GetSprite();
    craftedObject.Find("Item/Rarity/Quantity").GetComponent<TextMeshProUGUI>().text = quantity.ToString();

    var closeButton = craftedObject.Find("Buttons/Close").GetComponent<Button>();
    closeButton.onClick.RemoveAllListeners();
    closeButton.onClick.AddListener(() =>
    {
      CharacterProfileScript.instance.Load();
      craftedItemCanvasGroup.alpha = 0;
      craftedItemCanvasGroup.interactable = false;
      craftedItemCanvasGroup.blocksRaycasts = false;
      var canvasGroup = GetComponent<CanvasGroup>();
      canvasGroup.interactable = true;
      canvasGroup.blocksRaycasts = true;
    });
  }

  void FilterList()
  {
    filteredCrafts = crafts.Where(craft =>
    {
      if (craft.equipementBase != null)
        return craft.equipementBase.name.ToLower().Contains(filter.ToLower());
      return craft.item.name.ToLower().Contains(filter.ToLower());
    }).ToList();
  }

  void FilterList(string value)
  {
    filter = value;
    ReloadArtisanatList();
  }

  public async void Load()
  {
    LoadCrafts();
    await LoadInventory();
    ReloadArtisanatList();
  }

  void ReloadArtisanatList()
  {
    FilterList();
    ClearCraftsList();
    AddCrafts();
  }
}
