using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PortalDescription : MonoBehaviour
{
    [SerializeField]
    private GameObject warning;
    [SerializeField]
    private TextMeshProUGUI warningText;
    [SerializeField]
    private TextMeshProUGUI description;
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private Button enterButton;

    private POIScript poi;

    private string[] activitiesDescriptions = {
        "Détruit après l’invasion de la légion des Slimes, ce donjon servait d’oubliette pour les hors-la-loi sous le règne du roi Kiékié II.",
        "La fôret enchantée est un lieu magique où les arbres sont vivants et les animaux parlent. C’est un lieu de paix et de sérénité.",
        "Les buissons renferment des créatures étranges venuent d'un autre monde, Pokémon. Attention, vous trouverez peiut-être un Shiny !",
        "Mont cristal est une mine remplie de crystal où vie Myla une mineuse dont l'infeciton d'Hallownest a transformé en une créature monstrueuse."
    };

    public void Hide()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        poi = null;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        warning.SetActive(false);
        enterButton.interactable = true;
    }

    public void Enter()
    {
        if (!poi)
            return;

        if (!poi.IsPOIAvailable())
        {
            enterButton.interactable = false;
            warningText.text = "Ce portail n'est pas disponible pour le moment";
            warning.gameObject.SetActive(true);
        }
        else if (!poi.IsInRange())
        {
            enterButton.interactable = false;
            warningText.text = "Rapprochez-vous du portail pour pouvoir y acc�der";
            warning.gameObject.SetActive(true);
        }
        else
        {
            poi.UsePOI();
            Hide();
        }
    }

    public void Show(POIScript poi, string title)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        this.poi = poi;
        this.title.text = title;
        this.description.text = activitiesDescriptions[poi.GetAcitivity()];
    }
}
