using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PortalDescription : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI distanceWarning;
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
        distanceWarning.gameObject.SetActive(false);
    }

    public void Enter()
    {
        if (!poi)
            return;

        if (!poi.IsInRange())
        {
            distanceWarning.gameObject.SetActive(true);
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
