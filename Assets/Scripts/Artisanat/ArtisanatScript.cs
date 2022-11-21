using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Linq;
using RunPG.Multi;
using UnityEngine.UI;

public class ArtisanatScript : MonoBehaviour
{
    [SerializeField]
    private Transform craftsLayout;
    [SerializeField]
    private GameObject craftPrefab;
    [SerializeField]
    private TMP_InputField textInput;
    [SerializeField]
    private CanvasGroup confirmationCanvasGroup;
    [SerializeField]
    private CanvasGroup craftedObjectCanvasGroup;

    private string filter;
    
    private List<CraftModel> crafts;
    private List<CraftModel> filteredCrafts;

    void Start()
    {
        textInput.onValueChanged.AddListener(delegate
        {
            FilterList(textInput.text);
        });
        filter = textInput.text;
    }

    void LoadCrafts()
    {
        crafts = JsonUtility.FromJson<CraftModel[]>(Resources.Load<TextAsset>("Artisanat/Crafts").text).ToList();
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
        if (filteredCrafts.Count > 0)
        {
            Instantiate(craftPrefab, craftsLayout);
        }
        foreach (var craft in filteredCrafts)
        {
            var craftObject = Instantiate(craftPrefab, craftsLayout).transform;
            craftObject.Find("Name").GetComponent<TextMeshProUGUI>().text = craft.equipementBase.name;
            craftObject.Find("Golds").GetComponent<TextMeshProUGUI>().text = craft.golds.ToString();
            craftObject.Find("Crystals").GetComponent<TextMeshProUGUI>().text = craft.golds.ToString();
            craftObject.Find("Craft").GetComponent<Button>().onClick.AddListener(() => Craft(craft));
        }
    }

    void Craft(CraftModel craft)
    {
        confirmationCanvasGroup.alpha = 1;
        confirmationCanvasGroup.interactable = true;
        confirmationCanvasGroup.blocksRaycasts = true;
        confirmationCanvasGroup.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(async () =>
        {
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
        NewEquipementModel equipment = new NewEquipementModel(equipmentBase.id.ToString(), statistics);
        await Requests.POSTInventoryEquipement(PlayerProfile.id, equipment);
        return equipment;
    }

    void showCraftedObject(EquipmentBaseModel equipmentBase, StatisticsModel statistics)
    {
        craftedObjectCanvasGroup.alpha = 1;
        craftedObjectCanvasGroup.interactable = true;
        craftedObjectCanvasGroup.blocksRaycasts = true;
        Transform craftedObject = craftedObjectCanvasGroup.transform;
        craftedObject.Find("Name").GetComponent<TextMeshProUGUI>().text = equipmentBase.name;
        craftedObject.Find("ObjectImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("Artisanat/Objects/" + equipmentBase.id);
        craftedObject.Find("Statistics/vitalityValue").GetComponent<TextMeshProUGUI>().text = statistics.vitality.ToString();
        craftedObject.Find("Statistics/strengthValue").GetComponent<TextMeshProUGUI>().text = statistics.strength.ToString();
        craftedObject.Find("Statistics/defenseValue").GetComponent<TextMeshProUGUI>().text = statistics.defense.ToString();
        craftedObject.Find("Statistics/powerValue").GetComponent<TextMeshProUGUI>().text = statistics.power.ToString();
        craftedObject.Find("Statistics/resistanceValue").GetComponent<TextMeshProUGUI>().text = statistics.resistance.ToString();
        craftedObject.Find("Statistics/precisionValue").GetComponent<TextMeshProUGUI>().text = statistics.precision.ToString();
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

    public void Load()
    {
        LoadCrafts();
        ReloadGuildList();
    }

    void ReloadGuildList()
    {
        FilterList();
        ClearCraftsList();
        AddCrafts();
    }
}
