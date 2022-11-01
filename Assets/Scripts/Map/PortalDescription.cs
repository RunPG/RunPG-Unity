using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PortalDescription : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI warning;
    [SerializeField]
    private TextMeshProUGUI title;

    private POIScript poi;

    public void Hide()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        poi = null;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        warning.gameObject.SetActive(false);
    }

    public void Enter()
    {
        if (!poi)
            return;

        if (!poi.IsPOIAvailable())
        {
            warning.text = "Ce portail n'est pas disponible pour le moment";
            warning.gameObject.SetActive(true);
        }
        else if (!poi.IsInRange())
        {
            warning.text = "Rapprochez-vous du portail pour pouvoir y accéder";
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
    }
}
