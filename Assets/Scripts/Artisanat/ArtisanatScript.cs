using System.Collections;
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
  [SerializeField]
  private CanvasGroup confirmationCanvasGroup;
  [SerializeField]
  private CanvasGroup craftedObjectCanvasGroup;

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
      craftObject.Find("CraftIcon").GetComponent<Image>().sprite = craft.equipementBase.equipmentType.GetSprite();
      craftObject.Find("Name").GetComponent<TextMeshProUGUI>().text = craft.equipementBase.name;
      var materialsTransform = craftObject.Find("Materials").transform;
      bool canCraft = true;
      foreach (var material in craft.materials)
      {
        var foundMaterial = inventory.Find(x => x.itemId == material.id);
        bool hasMaterials = foundMaterial != null && foundMaterial.stackSize >= material.quantity;
        var materialObject = Instantiate(materialPrefab, materialsTransform).transform;
        materialObject.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Artisanat/Materials/" + material.id);
        materialObject.Find("Quantity").GetComponent<TextMeshProUGUI>().text = material.quantity.ToString();
        if (!hasMaterials)
        {
          materialObject.Find("Quantity").GetComponent<TextMeshProUGUI>().color = Color.red;
          canCraft = false;
        }
      }
      var craftButton = craftObject.Find("Craft").GetComponent<Button>();
      craftButton.onClick.AddListener(() => Craft(craft));
      craftButton.interactable = canCraft;
    }
  }

  void Craft(CraftModel craft)
  {
    var canvasGroup = GetComponent<CanvasGroup>();
    canvasGroup.interactable = false;
    canvasGroup.blocksRaycasts = false;

    confirmationCanvasGroup.alpha = 1;
    confirmationCanvasGroup.interactable = true;
    confirmationCanvasGroup.blocksRaycasts = true;

    confirmationCanvasGroup.transform.Find("Background/Name").GetComponent<TextMeshProUGUI>().text = craft.equipementBase.name;

    var materialGrid = confirmationCanvasGroup.transform.Find("Background/Materials").transform;
    foreach (Transform child in materialGrid)
    {
      Destroy(child.gameObject);
    }
    foreach (CraftItemModel material in craft.materials)
    {
      var materialObject = Instantiate(materialPrefab, materialGrid).transform;
      materialObject.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Artisanat/Materials/" + material.id);
      materialObject.Find("Quantity").GetComponent<TextMeshProUGUI>().text = material.quantity.ToString();
    }

    confirmationCanvasGroup.transform.Find("Background/Confirm").GetComponent<Button>().onClick.AddListener(async () =>
    {
      await DeduceMaterials(craft);
      NewEquipementModel equipment = await CraftNewEquipement(craft.equipementBase);
      confirmationCanvasGroup.alpha = 0;
      confirmationCanvasGroup.interactable = false;
      confirmationCanvasGroup.blocksRaycasts = false;
      showCraftedObject(craft.equipementBase, equipment.statistics);
    });
  }

  async Task<NewEquipementModel> CraftNewEquipement(EquipmentBaseModel equipmentBase)
  {
    StatisticsModel statistics = new StatisticsModel(0, 10, 10, 10, 10, 10, 10, 10);
    NewEquipementModel equipment = new NewEquipementModel(equipmentBase.id, statistics);
    await Requests.POSTInventoryEquipement(PlayerProfile.id, equipment);
    return equipment;
  }

  async Task DeduceMaterials(CraftModel craft)
  {
    foreach (var material in craft.materials)
    {
      await Requests.POSTInventoryItem(PlayerProfile.id, new PostItemModel(material.id, -material.quantity));
    }
  }

  void showCraftedObject(EquipmentBaseModel equipmentBase, StatisticsModel statistics)
  {
    craftedObjectCanvasGroup.alpha = 1;
    craftedObjectCanvasGroup.interactable = true;
    craftedObjectCanvasGroup.blocksRaycasts = true;
    Transform craftedObject = craftedObjectCanvasGroup.transform;
    craftedObject.Find("Background/Name").GetComponent<TextMeshProUGUI>().text = equipmentBase.name;
    // craftedObject.Find("ObjectImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("Artisanat/Objects/" + equipmentBase.id);
    craftedObject.Find("Background/Vitality/Value").GetComponent<TextMeshProUGUI>().text = statistics.vitality.ToString();
    craftedObject.Find("Background/Strength/Value").GetComponent<TextMeshProUGUI>().text = statistics.strength.ToString();
    craftedObject.Find("Background/Defense/Value").GetComponent<TextMeshProUGUI>().text = statistics.defense.ToString();
    craftedObject.Find("Background/Power/Value").GetComponent<TextMeshProUGUI>().text = statistics.power.ToString();
    craftedObject.Find("Background/Resistance/Value").GetComponent<TextMeshProUGUI>().text = statistics.resistance.ToString();
    craftedObject.Find("Background/Precision/Value").GetComponent<TextMeshProUGUI>().text = statistics.precision.ToString();
    craftedObject.GetComponent<Button>().onClick.AddListener(() =>
    {
      craftedObjectCanvasGroup.alpha = 0;
      craftedObjectCanvasGroup.interactable = false;
      craftedObjectCanvasGroup.blocksRaycasts = false;
      var canvasGroup = GetComponent<CanvasGroup>();
      canvasGroup.interactable = true;
      canvasGroup.blocksRaycasts = true;
    });
  }

  void FilterList()
  {
    filteredCrafts = crafts.Where(craft => craft.equipementBase.name.ToLower().Contains(filter.ToLower())).ToList();
  }

  void FilterList(string value)
  {
    filter = value;
    ReloadGuildList();
  }

  public async void Load()
  {
    LoadCrafts();
    await LoadInventory();
    ReloadGuildList();
  }

  void ReloadGuildList()
  {
    FilterList();
    ClearCraftsList();
    AddCrafts();
  }
}
