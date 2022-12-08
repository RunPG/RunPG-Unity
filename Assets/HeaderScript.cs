using System.Collections;
using System.Collections.Generic;
using RunPG.Multi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeaderScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI xpText;
    [SerializeField]
    private Slider xpBarSlider;
    [SerializeField]
    private TextMeshProUGUI caloriesText;
    [SerializeField]
    private TextMeshProUGUI goldText;

    public static HeaderScript instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            InvokeRepeating(nameof(UpdateHeader), 0, 30);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    public async void  UpdateHeader()
    {
        await Requests.PUTXp(PlayerProfile.id);
        await PlayerProfile.characterInfo.updateHeaderInformations(PlayerProfile.id);

        var currentXP = PlayerProfile.characterInfo.experience;
        var maxXP = PlayerProfile.characterInfo.GetRequiredExperience();

        goldText.text = PlayerProfile.characterInfo.gold.ToString();
        caloriesText.text = PlayerProfile.characterInfo.calories.ToString();
        xpText.text = string.Format("{0} / {1}", currentXP, maxXP);
        xpBarSlider.value = ((float)currentXP * 100) / maxXP;
        levelText.text = PlayerProfile.characterInfo.level.ToString();
    }
}
