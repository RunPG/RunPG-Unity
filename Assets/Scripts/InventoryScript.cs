using RunPG.Multi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Rarity;

public class InventoryScript : MonoBehaviour
{
    [Header("Header Informations")]
    [SerializeField]
    private TextMeshProUGUI usernameTextMesh;

    [SerializeField]
    private TextMeshProUGUI levelClasseTextMesh;

    [SerializeField]
    private Slider xpBarSlider;
    [SerializeField]
    private TextMeshProUGUI xpTextMesh;


    [Space(10)]
    [Header("Statistics Values")]
    [SerializeField]
    private TextMeshProUGUI vitalityTextMesh;
    [SerializeField]
    private TextMeshProUGUI forceTextMesh;
    [SerializeField]
    private TextMeshProUGUI defenseTextMesh;
    [SerializeField]
    private TextMeshProUGUI puissanceTextMesh;
    [SerializeField]
    private TextMeshProUGUI resistanceTextMesh;
    [SerializeField]
    private TextMeshProUGUI precisionTextMesh;
    [SerializeField]
    private TextMeshProUGUI agiliteTextMesh;


    [Space(10)]
    [Header("Classe")]
    [SerializeField]
    private Image classeImage;


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


    [Space(10)]
    [Header("Inventory")]
    [SerializeField]
    private GameObject inventoryLayout;
    [SerializeField]
    private GameObject equippedItemPrefab;
    [SerializeField]
    private GameObject itemPrefab;

    private GameObject selectedFilterBackground;
    private List<List<Item>> items;

    private EquipementModel[] equipements;

    private CharacterModel character;


    // Start is called before the first frame update
    async void Start()
    {
        selectedFilterBackground = weaponBackground;
        items = new List<List<Item>>();

        for (int i = 0; i < 7; i++)
            items.Add(new List<Item>());

        if (PlayerProfile.id != -1)
        {
            InventoryModel[] inventory = await Requests.GETUserInventory(PlayerProfile.id);
            character = await Requests.GETUserCharacter(PlayerProfile.id);
            PlayerProfile.character = character;

            foreach (InventoryModel inventoryItem in inventory)
            {
                if (inventoryItem.equipementId != null)
                {
                    var equipement = await Requests.GETEquipementById(inventoryItem.equipementId ?? 1);
                    AddEquipement(equipement);
                }
                else if (inventoryItem.itemId != null)
                {
                    //TODO
                }
            }

            AddEquipement(await Requests.GETEquipementById(character.weaponId), true);
            AddEquipement(await Requests.GETEquipementById(character.helmetId), true);
            AddEquipement(await Requests.GETEquipementById(character.chestplateId), true);
            AddEquipement(await Requests.GETEquipementById(character.glovesId), true);
            AddEquipement(await Requests.GETEquipementById(character.leggingsId), true);

            SetUsername(PlayerProfile.pseudo);
            SetLevelClasse(15, character.heroClass);
            SetVitality(10);
            SetForce(20);
            SetDefense(5);
            SetResistance(7);
            SetPuissance(15);
            SetAgilite(30);
            SetPrecision(2);
            SetImage(character.heroClass);

            SetXp(400, 960);
        }

        LoadSortedInventory(0);
        

        weaponButton.onClick.AddListener(delegate {
            SelectFilter(weaponBackground);
            LoadSortedInventory(0);
        });

        helmetButton.onClick.AddListener(delegate {
            SelectFilter(helmetBackground);
            LoadSortedInventory(1);
        });

        chestButton.onClick.AddListener(delegate {
            SelectFilter(chestBackground);
            LoadSortedInventory(2);
        });

        glovesButton.onClick.AddListener(delegate {
            SelectFilter(glovesBackground);
            LoadSortedInventory(3);
        });

        bootsButton.onClick.AddListener(delegate {
            SelectFilter(bootsBackground);
            LoadSortedInventory(4);
        });

        consumablesButton.onClick.AddListener(delegate {
            SelectFilter(consumablesBackground);
            LoadSortedInventory(5);
        });

        ressourcesButton.onClick.AddListener(delegate {
            SelectFilter(ressourcesBackground);
            LoadSortedInventory(6);
        });
    }

    void LoadInventory(int filterIndex)
    {
        foreach (Transform child in inventoryLayout.transform)
        {
            Destroy(child.gameObject);
        }

        var filterList = items[filterIndex];

        if (filterList.Count == 0)
            inventoryLayout.GetComponent<VerticalLayoutGroup>().padding.bottom = 0;
        else
            inventoryLayout.GetComponent<VerticalLayoutGroup>().padding.bottom = 8;

        for (int itemIndex = 0; itemIndex < filterList.Count; itemIndex++)
        {
            var item = filterList[itemIndex];
            var newItem = Instantiate(item.equiped ? equippedItemPrefab : itemPrefab, inventoryLayout.transform).transform;
            newItem.Find("Image").GetComponent<Image>().sprite = item.sprite;
            newItem.Find("Name").GetComponent<TextMeshProUGUI>().text = item.name;
            newItem.Find("Level").GetComponent<TextMeshProUGUI>().text = string.Format("Lv. {0}", item.level);

            var rarity = newItem.Find("Rarity").GetComponent<TextMeshProUGUI>();
            rarity.text = item.rarity.GetName();
            rarity.color = item.rarity.GetColor();

            if (!item.equiped)
            {
                var itemIndexCopy = itemIndex;
                newItem.Find("Button").GetComponent<Button>().onClick.AddListener(delegate {
                    var selectedIndex = filterList.FindIndex((item) => { return item.equiped; });
                    if (selectedIndex != -1)
                    {
                        var selectedItem = filterList[selectedIndex];
                        selectedItem.equiped = false;
                        filterList[selectedIndex] = selectedItem;
                    }
                    item.equiped = true;
                    filterList[itemIndexCopy] = item;
                    LoadInventory(filterIndex);
                });
            }
        }
    }

    private void LoadSortedInventory(int filterIndex)
    {
        var filtereditems = items[filterIndex];
        filtereditems.Sort((a, b) =>
        {
            if (a.equiped)
                return -1;
            if (b.equiped)
                return 1;
            if (b.level - a.level != 0)
                return b.level - a.level;
            return b.rarity - a.rarity;
        });

        LoadInventory(filterIndex);
    }

    public void ResetInventory()
    {
        SelectFilter(weaponBackground);
        LoadSortedInventory(0);
    }

    void SelectFilter(GameObject backgroundFilter)
    {
        var previousRectTransform = selectedFilterBackground.GetComponent<RectTransform>();
        previousRectTransform.offsetMin = new Vector2(previousRectTransform.offsetMin.x, 10);

        var acutalRectTransform = backgroundFilter.GetComponent<RectTransform>();
        acutalRectTransform.offsetMin = new Vector2(acutalRectTransform.offsetMin.x, 0);
        selectedFilterBackground = backgroundFilter;
    }

    void SetUsername(string username)
    {
        usernameTextMesh.text = username;
    }

    void SetLevelClasse(int level, HeroClass classe)
    {        
        levelClasseTextMesh.text = string.Format("Lv.{0} - {1}", level, classe.GetName());
    }

    void SetXp(int currentXP, int maxXP)
    {
        xpTextMesh.text = string.Format("{0} / {1}", currentXP, maxXP);
        xpBarSlider.value = currentXP * 100 / maxXP;
    }

    void SetVitality(int vitality)
    {
        vitalityTextMesh.text = vitality.ToString();
    }

    void SetForce(int force)
    {
        forceTextMesh.text = force.ToString();
    }

    void SetDefense(int defense)
    {
        defenseTextMesh.text = defense.ToString();
    }

    void SetPuissance(int puissance)
    {
        puissanceTextMesh.text = puissance.ToString();
    }

    void SetResistance(int resistance)
    {
        resistanceTextMesh.text = resistance.ToString();
    }

    void SetPrecision(int precision)
    {
        precisionTextMesh.text = precision.ToString();
    }

    void SetAgilite(int agilite)
    {
        agiliteTextMesh.text = agilite.ToString();
    }

    void SetImage(HeroClass classe)
    {
        classeImage.sprite = classe.GetSprite();
    }

    void AddEquipement(EquipementModel equipement, bool equiped =false)
    {
        var equipementBase = equipement.equipementBase;
        var equipementType = equipementBase.equipementType;
        var rarity = equipementBase.rarity;
        Item newItem = new()
        {
            equiped = equiped,
            level = equipement.statistics.level,
            name = equipement.equipementBase.name,
            rarity = rarity,
            sprite = equipementType.GetSprite()
        };
        items[equipementType.GetIndex()].Add(newItem);
    }
}

public struct Item
{
    public string name { get; set; }
    public Sprite sprite { get; set; }
    public Rarity rarity { get; set; }
    public int level { get; set; }
    public bool equiped { get; set; }
}