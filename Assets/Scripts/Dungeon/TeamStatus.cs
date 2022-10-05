using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DungeonManager;

public class TeamStatus : MonoBehaviour
{
    [SerializeField]
    private GameObject statusPrefab;

    [SerializeField]
    private Transform layout;

    // Start is called before the first frame update
    public void LoadStatus()
    {
        foreach (Transform child in layout.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (DungeonCharacterInfo character in DungeonManager.instance.characters)
        {
            var newStatus = Instantiate(statusPrefab, layout).transform;
            newStatus.Find("Username").GetComponent<TextMeshProUGUI>().text = character.name;
            newStatus.Find("HpBar").GetComponent<Slider>().value = character.ratioHP;
            int maxHP = character.stats.GetMaxHp(character.level);
            int currentHP = Mathf.RoundToInt(character.ratioHP * maxHP);
            newStatus.Find("Hp").GetComponent<TextMeshProUGUI>().text = string.Format("{0} / {1} hp", currentHP, maxHP);
            newStatus.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("Classe/{0}", character.classType));
        }
    }
}
