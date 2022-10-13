using RunPG.Multi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Rarity;

public class CharacterProfileScript : MonoBehaviour
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
    private GameObject equippedItemPrefab;
    [SerializeField]
    private GameObject itemPrefab;

    private GameObject selectedFilterBackground;
    private List<List<Equipment>> equipments;

    // Start is called before the first frame update
    async void Start()
    {
        selectedFilterBackground = weaponBackground;
        equipments = new List<List<Equipment>>();

        for (int i = 0; i < 7; i++)
            equipments.Add(new List<Equipment>());

        if (PlayerProfile.id != -1)
        {
            InventoryModel[] inventory = await Requests.GETUserInventory(PlayerProfile.id);
            PlayerProfile.characterInfo = await CharacterInfo.Load(PlayerProfile.id);

            foreach (InventoryModel inventoryItem in inventory)
            {
                if (inventoryItem.equipementId.HasValue)
                {
                    var equipmentModel = await Requests.GETEquipementById(inventoryItem.equipementId.Value);
                    var equipment = new Equipment(equipmentModel);
                    equipments[equipment.type.GetIndex()].Add(equipment);
                }
                else if (inventoryItem.itemId != null)
                {
                    //TODO
                }
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
            var newItem = Instantiate(IsEquiped(equipment) ? equippedItemPrefab : itemPrefab, inventoryLayout.transform).transform;
            newItem.Find("Image").GetComponent<Image>().sprite = equipment.type.GetSprite();
            newItem.Find("Name").GetComponent<TextMeshProUGUI>().text = equipment.name;
            newItem.Find("Level").GetComponent<TextMeshProUGUI>().text = string.Format("Lv. {0}", equipment.level);

            var rarity = newItem.Find("Rarity").GetComponent<TextMeshProUGUI>();
            rarity.text = equipment.rarity.GetName();
            rarity.color = equipment.rarity.GetColor();

            if (!IsEquiped(equipment))
            {
                var itemIndexCopy = equipmentIndex;
                Button button = newItem.Find("Button").GetComponent<Button>();
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
        LevelUpButton.SetActive(currentXP >= maxXP);
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
        levelClasseTextMesh.text = string.Format("Lv.{0} - {1}", level, classe.GetName());
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
}