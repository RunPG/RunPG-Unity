using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    [Space(10)]
    [SerializeField]
    private CanvasGroup popUp;
    [SerializeField]
    private ProfileScript profile;

    [Space(20)]
    [Header("Plus Buttons")]
    [SerializeField]
    private Button vitalityPlusButton;
    [SerializeField]
    private Button strengthPlusButton;
    [SerializeField]
    private Button defensePlusButton;
    [SerializeField]
    private Button powerPlusButton;
    [SerializeField]
    private Button resistancePlusButton;
    [SerializeField]
    private Button precisionPlusButton;

    [Space(20)]
    [Header("Minus Buttons")]
    [SerializeField]
    private Button vitalityMinusButton;
    [SerializeField]
    private Button strengthMinusButton;
    [SerializeField]
    private Button defenseMinusButton;
    [SerializeField]
    private Button powerMinusButton;
    [SerializeField]
    private Button resistanceMinusButton;
    [SerializeField]
    private Button precisionMinusButton;

    [Space(20)]
    [Header("Value Texts")]
    [SerializeField]
    private TextMeshProUGUI vitalityText;
    [SerializeField]
    private TextMeshProUGUI strengthText;
    [SerializeField]
    private TextMeshProUGUI defenseText;
    [SerializeField]
    private TextMeshProUGUI powerText;
    [SerializeField]
    private TextMeshProUGUI resistanceText;
    [SerializeField]
    private TextMeshProUGUI precisionText;

    [Space(20)]
    [SerializeField]
    private Button completeButton;

    [Space(20)]
    [SerializeField]
    private TextMeshProUGUI pointsText;

    private int numberLevelUp;
    private int futureXp;

    private int vitalityIncrement;
    private int strengthIncrement;
    private int defenseIncrement;
    private int powerIncrement;
    private int resistanceIncrement;
    private int precisionIncrement;


    public void ShowPopUp(bool state)
    {
        popUp.alpha = state ? 1f : 0f;
        popUp.interactable = state;
        popUp.blocksRaycasts = state;
    }

    public void OpenLevelUp()
    {
        CalculateLevelUp();
        RefreshButtons();
        UpdatePointsText();
        vitalityText.text = PlayerProfile.characterInfo.vitality.ToString();
        strengthText.text = PlayerProfile.characterInfo.strength.ToString();
        defenseText.text = PlayerProfile.characterInfo.defense.ToString();
        powerText.text = PlayerProfile.characterInfo.power.ToString();
        resistanceText.text = PlayerProfile.characterInfo.resistance.ToString();
        precisionText.text = PlayerProfile.characterInfo.precision.ToString();
        ShowPopUp(true);
    }

    public void CompleteLevelUp()
    {
        PlayerProfile.characterInfo.level += numberLevelUp;
        PlayerProfile.characterInfo.experience = futureXp;
        PlayerProfile.characterInfo.vitality += vitalityIncrement;
        PlayerProfile.characterInfo.strength += strengthIncrement;
        PlayerProfile.characterInfo.defense += defenseIncrement;
        PlayerProfile.characterInfo.power += powerIncrement;
        PlayerProfile.characterInfo.resistance += resistanceIncrement;
        PlayerProfile.characterInfo.precision += precisionIncrement;
        profile.RefreshStat();
        profile.RefreshXp();
        ShowPopUp(false);
    }

    private void CalculateLevelUp()
    {
        futureXp = PlayerProfile.characterInfo.experience;
        numberLevelUp = 0;
        int requiredXp = PlayerProfile.characterInfo.GetRequiredExperience();
        while (futureXp >= requiredXp)
        {
            futureXp -= requiredXp;
            numberLevelUp++;
            requiredXp = PlayerProfile.characterInfo.GetRequiredExperience(PlayerProfile.characterInfo.level + numberLevelUp);
        }
    }

    public void IncreaseVitality()
    {
        vitalityIncrement++;
        vitalityText.text = (PlayerProfile.characterInfo.vitality + vitalityIncrement).ToString();
        RefreshButtons();
        UpdatePointsText();
    }

    public void DecreaseVitality()
    {
        vitalityIncrement--;
        vitalityText.text = (PlayerProfile.characterInfo.vitality + vitalityIncrement).ToString();
        RefreshButtons();
        UpdatePointsText();
    }

    public void IncreaseStrength()
    {
        strengthIncrement++;
        strengthText.text = (PlayerProfile.characterInfo.strength + strengthIncrement).ToString();
        RefreshButtons();
        UpdatePointsText();
    }

    public void DecreaseStrength()
    {
        strengthIncrement--;
        strengthText.text = (PlayerProfile.characterInfo.strength + strengthIncrement).ToString();
        RefreshButtons();
        UpdatePointsText();
    }

    public void IncreaseDefense()
    {
        defenseIncrement++;
        defenseText.text = (PlayerProfile.characterInfo.defense + defenseIncrement).ToString();
        RefreshButtons();
        UpdatePointsText();
    }

    public void DecreaseDefense()
    {
        defenseIncrement--;
        defenseText.text = (PlayerProfile.characterInfo.defense + defenseIncrement).ToString();
        RefreshButtons();
        UpdatePointsText();
    }

    public void IncreasePower()
    {
        powerIncrement++;
        powerText.text = (PlayerProfile.characterInfo.power + powerIncrement).ToString();
        RefreshButtons();
        UpdatePointsText();
    }

    public void DecreasePower()
    {
        powerIncrement--;
        powerText.text = (PlayerProfile.characterInfo.power + powerIncrement).ToString();
        RefreshButtons();
        UpdatePointsText();
    }

    public void IncreaseResistance()
    {
        resistanceIncrement++;
        resistanceText.text = (PlayerProfile.characterInfo.resistance + resistanceIncrement).ToString();
        RefreshButtons();
        UpdatePointsText();
    }

    public void DecreaseResistance()
    {
        resistanceIncrement--;
        resistanceText.text = (PlayerProfile.characterInfo.resistance + resistanceIncrement).ToString();
        RefreshButtons();
        UpdatePointsText();
    }

    public void IncreasePrecision()
    {
        precisionIncrement++;
        precisionText.text = (PlayerProfile.characterInfo.precision + precisionIncrement).ToString();
        RefreshButtons();
        UpdatePointsText();
    }

    public void DecreasePrecision()
    {
        precisionIncrement--;
        precisionText.text = (PlayerProfile.characterInfo.precision + precisionIncrement).ToString();
        RefreshButtons();
        UpdatePointsText();
    }

    private void RefreshButtons()
    {
        bool state = (numberLevelUp * 4) - (vitalityIncrement + strengthIncrement + defenseIncrement + powerIncrement + resistanceIncrement + precisionIncrement) > 0;
        vitalityPlusButton.enabled = state;
        strengthPlusButton.enabled = state;
        defensePlusButton.enabled = state;
        powerPlusButton.enabled = state;
        resistancePlusButton.enabled = state;
        precisionPlusButton.enabled = state;

        vitalityMinusButton.enabled = vitalityIncrement > 0;
        strengthMinusButton.enabled = strengthIncrement > 0;
        defenseMinusButton.enabled = defenseIncrement > 0;
        powerMinusButton.enabled = powerIncrement > 0;
        resistanceMinusButton.enabled = resistanceIncrement > 0;
        precisionMinusButton.enabled = precisionIncrement > 0;

        completeButton.enabled = !state;
    }

    private void UpdatePointsText()
    {
        pointsText.text = "<color=#FF9B42>" + ((numberLevelUp * 4) - (vitalityIncrement + strengthIncrement + defenseIncrement + powerIncrement + resistanceIncrement + precisionIncrement)).ToString() + "</color> point(s) disponible(s)";
    }
}
