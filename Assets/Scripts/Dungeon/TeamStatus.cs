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
            newStatus.Find("XpBar").GetComponent<Slider>().value = character.currentHP * 100 / character.maxHP;
            newStatus.Find("Xp").GetComponent<TextMeshProUGUI>().text = string.Format("{0} / {1} hp", character.currentHP, character.maxHP); 
        }
    }
}
