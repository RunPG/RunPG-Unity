using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
public class MarketPlace : MonoBehaviour
{
    [SerializeField]
    private OfferEquipmentDisplay OfferEquipmentDisplay;
    [SerializeField]
    private Transform prefabPos;
    [SerializeField]
    private OfferEquipmentDescription offerEquipmentDescription;
    /* [SerializeField]
     private OfferItemDisplay OfferItemDisplay;*/
    // Start is called before the first frame update
    async void Start()
    {
        var marketModels = await Requests.GETallOpenItems();
        foreach (var marketModel in marketModels)
        {
            Debug.Log(marketModel.id);
            if (!marketModel.isSold)
            {
                if (marketModel.equipmentId.HasValue)
                {
                    var prefab = Instantiate(OfferEquipmentDisplay, prefabPos);
                    var equipmentModel = await Requests.GETEquipmentById(marketModel.equipmentId.Value);
                    var equipment = new Equipment(equipmentModel);
                    prefab.SetInformations(marketModel, equipment);
                    
                    prefab.buyButton.onClick.AddListener(() => { offerEquipmentDescription.LoadPopUp(GetComponent<CanvasGroup>(), equipment, marketModel); });
                    
                }
                else
                {
                    /*var prefab = Instantiate(OfferItemDisplay, prefabPos);
                    var item = await Requests.GETEquipmentById(marketModel.itemId.Value);
                    prefab.SetInformations();*/
                     
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}