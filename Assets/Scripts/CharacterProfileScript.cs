using RunPG.Multi;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterProfileScript : MonoBehaviour
{
    [Header("Header Informations")]
    [SerializeField]
    private TextMeshProUGUI usernameTextMesh;

    [SerializeField]
    private TextMeshProUGUI classeTextMesh;
    [SerializeField]
    private TextMeshProUGUI levelTextMesh;

    [SerializeField]
    private Slider xpBarSlider;
    [SerializeField]
    private TextMeshProUGUI xpTextMesh;
    [SerializeField]
    private GameObject LevelUpButton;


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
    private GameObject equipmentPrefab;
    [SerializeField]
    private GameObject itemPrefab;

    private GameObject selectedFilterBackground;
    private List<List<Equipment>> equipments;

    [Space(10)]
    [Header("PopUp")]
    [SerializeField]
    private ObjectDescriptionScript objectDescriptionScript;

    public static CharacterProfileScript instance;

    // Start is called before the first frame update
    async void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
        selectedFilterBackground = weaponBackground;
        equipments = new List<List<Equipment>>();

        for (int i = 0; i < 7; i++)
            equipments.Add(new List<Equipment>());

        if (PlayerProfile.id != -1)
        {
            await Requests.PUTXp(PlayerProfile.id);
            InventoryModel[] inventory = await Requests.GETUserInventory(PlayerProfile.id);
            PlayerProfile.characterInfo = await CharacterInfo.Load(PlayerProfile.id);



            foreach (InventoryModel inventoryItem in inventory)
            {
                if (inventoryItem.equipmentId.HasValue)
                {
                    var equipmentModel = await Requests.GETEquipmentById(inventoryItem.equipmentId.Value);
                    var equipment = new Equipment(equipmentModel);
                    equipments[equipment.type.GetIndex()].Add(equipment);
                }
                else if (inventoryItem.itemId != null)
                {
                    var itemModel = await Requests.GetItemById(inventoryItem.itemId.Value);
                    var equipment = new Equipment(itemModel[0], inventoryItem.stackSize);
                    equipments[itemModel[0].isConsomable ? 5 : 6].Add(equipment);
                }
            }

            foreach (List<Equipment> equipmentList in equipments)
            {
                Debug.Log("count:" + equipmentList.Count);
            }

            SetUsername(PlayerProfile.pseudo);
            SetLevelClass(PlayerProfile.characterInfo.level, PlayerProfile.characterInfo.heroClass);
            RefreshXp();
            RefreshStat();
            SetImage(PlayerProfile.characterInfo.heroClass);
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

        var filterList = equipments[filterIndex];

        if (filterList.Count == 0)
            inventoryLayout.GetComponent<VerticalLayoutGroup>().padding.bottom = 0;
        else
            inventoryLayout.GetComponent<VerticalLayoutGroup>().padding.bottom = 8;

        for (int equipmentIndex = 0; equipmentIndex < filterList.Count; equipmentIndex++)
        {
            var equipment = filterList[equipmentIndex];

            if (equipment.isItem)
            {
                var newItem = Instantiate(itemPrefab, inventoryLayout.transform).transform;
                newItem.GetComponent<Image>().sprite = equipment.rarity.GetSprite();
                newItem.Find("Name").GetComponent<TextMeshProUGUI>().text = equipment.name;
                newItem.Find("Image").GetComponent<Image>().sprite = equipment.GetEquipmentSprite();
                newItem.Find("Quantity").GetComponent<TextMeshProUGUI>().text = equipment.stackSize.ToString();
                continue;
            }

            var equipedItem = GetEquiped(equipment.type);
            var newEquipment = Instantiate(equipmentPrefab, inventoryLayout.transform).transform;

            newEquipment.Find("Image").GetComponent<Image>().sprite = equipment.GetEquipmentSprite();
            newEquipment.Find("Name").GetComponent<TextMeshProUGUI>().text = equipment.name;

            newEquipment.GetComponent<Image>().sprite = equipment.rarity.GetSprite();
            
            var vitalityText = newEquipment.Find("Stats/Vitality/Value").GetComponent<TextMeshProUGUI>();
            vitalityText.text = equipment.vitality.ToString();
            vitalityText.color = GetStatisticColor(equipedItem.vitality, equipment.vitality);

            var strengthText = newEquipment.Find("Stats/Strength/Value").GetComponent<TextMeshProUGUI>();
            strengthText.text = equipment.strength.ToString();
            strengthText.color = GetStatisticColor(equipedItem.strength, equipment.strength);

            var defenseText = newEquipment.Find("Stats/Defense/Value").GetComponent<TextMeshProUGUI>();
            defenseText.text = equipment.defense.ToString();
            defenseText.color = GetStatisticColor(equipedItem.defense, equipment.defense);

            var resistanceText = newEquipment.Find("Stats/Resistance/Value").GetComponent<TextMeshProUGUI>();
            resistanceText.text = equipment.resistance.ToString();
            resistanceText.color = GetStatisticColor(equipedItem.resistance, equipment.resistance);

            var powerText = newEquipment.Find("Stats/Power/Value").GetComponent<TextMeshProUGUI>();
            powerText.text = equipment.power.ToString();
            powerText.color = GetStatisticColor(equipedItem.power, equipment.power);

            var precisionText = newEquipment.Find("Stats/Precision/Value").GetComponent<TextMeshProUGUI>();
            precisionText.text = equipment.precision.ToString();
            precisionText.color = GetStatisticColor(equipedItem.precision, equipment.precision);

            var isEquiped = equipedItem.id == equipment.id;

            newEquipment.Find("ShowDescription").GetComponent<Button>().onClick.AddListener(() => {
                objectDescriptionScript.LoadPopUp(GetComponent<CanvasGroup>(), equipment, isEquiped);
                });

            Button button = newEquipment.Find("Button").GetComponent<Button>();

            if (equipment.heroClass != PlayerProfile.characterInfo.heroClass)
            {
                button.gameObject.SetActive(false);
                newEquipment.Find("Level").GetComponent<TextMeshProUGUI>().text = string.Format("Nv. {0} - {1}", equipment.level, equipment.heroClass.ToString());
            }
            else if (isEquiped)
            {
                button.interactable = false;
                button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Equipé";
                newEquipment.Find("Level").GetComponent<TextMeshProUGUI>().text = string.Format("Nv. {0}", equipment.level);
            }
            else
            {
                var itemIndexCopy = equipmentIndex;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(async delegate {
                    PlayerEquipmentModel playerEquipment = new PlayerEquipmentModel(PlayerProfile.characterInfo.helmet.id, PlayerProfile.characterInfo.chestplate.id,
                        PlayerProfile.characterInfo.gloves.id, PlayerProfile.characterInfo.leggings.id, PlayerProfile.characterInfo.weapon.id);
                    switch (equipment.type)
                    {
                        case EquipmentType.WEAPON:
                            playerEquipment.weaponId = equipment.id;
                            if (!await Requests.POSTPlayerEquipment(PlayerProfile.id, playerEquipment))
                                return;
                            PlayerProfile.characterInfo.weapon = equipment;
                            break;
                        case EquipmentType.HELMET:
                            playerEquipment.helmetId = equipment.id;
                            if (!await Requests.POSTPlayerEquipment(PlayerProfile.id, playerEquipment))
                                return;
                            PlayerProfile.characterInfo.helmet = equipment;
                            break;
                        case EquipmentType.CHESTPLATE:
                            playerEquipment.chestplateId = equipment.id;
                            if (!await Requests.POSTPlayerEquipment(PlayerProfile.id, playerEquipment))
                                return;
                            PlayerProfile.characterInfo.chestplate = equipment;
                            break;
                        case EquipmentType.GLOVES:
                            playerEquipment.glovesId = equipment.id;
                            if (!await Requests.POSTPlayerEquipment(PlayerProfile.id, playerEquipment))
                                return;
                            PlayerProfile.characterInfo.gloves = equipment;
                            break;
                        case EquipmentType.LEGGINGS:
                            playerEquipment.leggingsId = equipment.id;
                            if (!await Requests.POSTPlayerEquipment(PlayerProfile.id, playerEquipment))
                                return;
                            PlayerProfile.characterInfo.leggings = equipment;
                            break;
                    }
                    RefreshStat();
                    LoadInventory(filterIndex);
                });
            }
        }
    }

    public async void Equip(Equipment equipment)
    {
        PlayerEquipmentModel playerEquipment = new PlayerEquipmentModel(PlayerProfile.characterInfo.helmet.id, PlayerProfile.characterInfo.chestplate.id,
            PlayerProfile.characterInfo.gloves.id, PlayerProfile.characterInfo.leggings.id, PlayerProfile.characterInfo.weapon.id);
        switch (equipment.type)
        {
            case EquipmentType.WEAPON:
                playerEquipment.weaponId = equipment.id;
                if (!await Requests.POSTPlayerEquipment(PlayerProfile.id, playerEquipment))
                    return;
                PlayerProfile.characterInfo.weapon = equipment;
                break;
            case EquipmentType.HELMET:
                playerEquipment.helmetId = equipment.id;
                if (!await Requests.POSTPlayerEquipment(PlayerProfile.id, playerEquipment))
                    return;
                PlayerProfile.characterInfo.helmet = equipment;
                break;
            case EquipmentType.CHESTPLATE:
                playerEquipment.chestplateId = equipment.id;
                if (!await Requests.POSTPlayerEquipment(PlayerProfile.id, playerEquipment))
                    return;
                PlayerProfile.characterInfo.chestplate = equipment;
                break;
            case EquipmentType.GLOVES:
                playerEquipment.glovesId = equipment.id;
                if (!await Requests.POSTPlayerEquipment(PlayerProfile.id, playerEquipment))
                    return;
                PlayerProfile.characterInfo.gloves = equipment;
                break;
            case EquipmentType.LEGGINGS:
                playerEquipment.leggingsId = equipment.id;
                if (!await Requests.POSTPlayerEquipment(PlayerProfile.id, playerEquipment))
                    return;
                PlayerProfile.characterInfo.leggings = equipment;
                break;
        }
        RefreshStat();
        LoadInventory(GetEquipmentIndex(equipment));
    }

    private void LoadSortedInventory(int filterIndex)
    {
        var filteredEquipments = equipments[filterIndex];
        filteredEquipments.Sort((a, b) =>
        {
            if (IsEquiped(a))
                return -1;
            if (IsEquiped(b))
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

    public void RefreshXp()
    {
        int currentXP = PlayerProfile.characterInfo.experience;
        int maxXP = PlayerProfile.characterInfo.GetRequiredExperience();
        xpTextMesh.text = string.Format("{0} / {1}", currentXP, maxXP);
        xpBarSlider.value = ((float)currentXP * 100) / maxXP;
        levelTextMesh.text = PlayerProfile.characterInfo.level.ToString();
        LevelUpButton.SetActive(currentXP >= maxXP);
        xpTextMesh.gameObject.SetActive(currentXP < maxXP);
    }

    public void RefreshStat()
    {
        SetVitality(PlayerProfile.characterInfo.GetTotalVitality());
        SetStrength(PlayerProfile.characterInfo.GetTotalStrength());
        SetDefense(PlayerProfile.characterInfo.GetTotalDefense());
        SetPower(PlayerProfile.characterInfo.GetTotalPower());
        SetResistance(PlayerProfile.characterInfo.GetTotalResistance());
        SetPrecision(PlayerProfile.characterInfo.GetTotalPrecision());
    }

    bool IsEquiped(Equipment equipment)
    {
        return equipment.type switch
        {
            EquipmentType.WEAPON => PlayerProfile.characterInfo.weapon != null && equipment.id == PlayerProfile.characterInfo.weapon.id,
            EquipmentType.HELMET => PlayerProfile.characterInfo.helmet != null &&  equipment.id == PlayerProfile.characterInfo.helmet.id,
            EquipmentType.CHESTPLATE => PlayerProfile.characterInfo.chestplate != null && equipment.id == PlayerProfile.characterInfo.chestplate.id,
            EquipmentType.GLOVES => PlayerProfile.characterInfo.gloves != null && equipment.id == PlayerProfile.characterInfo.gloves.id,
            EquipmentType.LEGGINGS => PlayerProfile.characterInfo.leggings != null && equipment.id == PlayerProfile.characterInfo.leggings.id,
            _ => false
        };
    }

    Equipment GetEquiped(EquipmentType type)
    {
        return type switch
        {
            EquipmentType.WEAPON => PlayerProfile.characterInfo.weapon,
            EquipmentType.HELMET => PlayerProfile.characterInfo.helmet,
            EquipmentType.CHESTPLATE => PlayerProfile.characterInfo.chestplate,
            EquipmentType.GLOVES => PlayerProfile.characterInfo.gloves,
            EquipmentType.LEGGINGS => PlayerProfile.characterInfo.leggings,
            _ => null
        };
    }

    Color GetStatisticColor(int equipedValue, int itemValue)
    {
        if (equipedValue == itemValue)
            return Color.white;
        return equipedValue > itemValue ? Color.red : Color.green;
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

    void SetLevelClass(int level, HeroClass classe)
    {
        classeTextMesh.text = string.Format(classe.GetName());
        levelTextMesh.text = string.Format(level.ToString());
    }

    void SetVitality(int vitality)
    {
        vitalityTextMesh.text = vitality.ToString();
    }

    void SetStrength(int force)
    {
        forceTextMesh.text = force.ToString();
    }

    void SetDefense(int defense)
    {
        defenseTextMesh.text = defense.ToString();
    }

    void SetPower(int puissance)
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

    void SetImage(HeroClass classe)
    {
        classeImage.sprite = classe.GetSprite();
    }

    private int GetEquipmentIndex(Equipment equipment)
    {
        switch (equipment.type)
        {
            case EquipmentType.WEAPON:
                return 0;
            case EquipmentType.HELMET:
                return 1;
            case EquipmentType.CHESTPLATE:
                return 2;
            case EquipmentType.GLOVES:
                return 3;
            case EquipmentType.LEGGINGS:
                return 4;
            default:
                return 0;
        }
    }
}