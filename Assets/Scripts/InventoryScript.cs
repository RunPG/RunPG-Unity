using System.Collections.Generic;
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


    // Start is called before the first frame update
    void Start()
    {
        selectedFilterBackground = weaponBackground;
        items = new List<List<Item>>();

        for (int i = 0; i < 7; i++)
            items.Add(new List<Item>());

        items[0].Add(new Item { equiped = true, level = 15, name = "Epée", rarity = RarityType.Legendary, sprite = Resources.Load<Sprite>("Inventory/sword") });
        items[2].Add(new Item { equiped = true, level = 15, name = "Chest", rarity = RarityType.Legendary, sprite = Resources.Load<Sprite>("Inventory/sword") });
        items[2].Add(new Item { equiped = false, level = 15, name = "Chest", rarity = RarityType.Legendary, sprite = Resources.Load<Sprite>("Inventory/sword") });
        items[2].Add(new Item { equiped = false, level = 15, name = "Chest", rarity = RarityType.Legendary, sprite = Resources.Load<Sprite>("Inventory/sword") });
        items[2].Add(new Item { equiped = false, level = 15, name = "Chest", rarity = RarityType.Legendary, sprite = Resources.Load<Sprite>("Inventory/sword") });
        items[2].Add(new Item { equiped = false, level = 15, name = "Chest", rarity = RarityType.Legendary, sprite = Resources.Load<Sprite>("Inventory/sword") });
        items[2].Add(new Item { equiped = false, level = 15, name = "Chest", rarity = RarityType.Legendary, sprite = Resources.Load<Sprite>("Inventory/sword") });
        items[2].Add(new Item { equiped = false, level = 15, name = "Chest", rarity = RarityType.Legendary, sprite = Resources.Load<Sprite>("Inventory/sword") });
        items[2].Add(new Item { equiped = false, level = 15, name = "Chest", rarity = RarityType.Legendary, sprite = Resources.Load<Sprite>("Inventory/sword") });
        items[2].Add(new Item { equiped = false, level = 15, name = "Chest", rarity = RarityType.Legendary, sprite = Resources.Load<Sprite>("Inventory/sword") });
        items[2].Add(new Item { equiped = false, level = 15, name = "Chest", rarity = RarityType.Legendary, sprite = Resources.Load<Sprite>("Inventory/sword") });
        LoadInventory(0);

        weaponButton.onClick.AddListener(delegate {
            SelectFilter(weaponBackground);
            LoadInventory(0);
        });

        helmetButton.onClick.AddListener(delegate {
            SelectFilter(helmetBackground);
            LoadInventory(1);
        });

        chestButton.onClick.AddListener(delegate {
            SelectFilter(chestBackground);
            LoadInventory(2);
        });

        glovesButton.onClick.AddListener(delegate {
            SelectFilter(glovesBackground);
            LoadInventory(3);
        });

        bootsButton.onClick.AddListener(delegate {
            SelectFilter(bootsBackground);
            LoadInventory(4);
        });

        consumablesButton.onClick.AddListener(delegate {
            SelectFilter(consumablesBackground);
            LoadInventory(5);
        });

        ressourcesButton.onClick.AddListener(delegate {
            SelectFilter(ressourcesBackground);
            LoadInventory(6);
        });

        SetUsername("ZenSheep");
        SetLevelClasse(5, "Paladin");
        SetVitality(10);
        SetForce(20);
        SetDefense(5);
        SetResistance(7);
        SetPuissance(15);
        SetAgilite(30);
        SetPrecision(2);

        SetXp(400, 960);
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
            rarity.text = item.rarity.ToString();
            rarity.color = getRarityColor(item.rarity);

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

    void SetLevelClasse(int level, string classe)
    {
        levelClasseTextMesh.text = string.Format("Lv.{0} - {1}", level, classe);
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
}

public struct Item
{
    public string name { get; set; }
    public Sprite sprite { get; set; }
    public RarityType rarity { get; set; }
    public int level { get; set; }
    public bool equiped { get; set; }
}