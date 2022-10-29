using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateGuildScript : MonoBehaviour
{
    [SerializeField]
    private GameObject textInputObject;
    [SerializeField]
    private GameObject guildPageCanvas;
    [SerializeField]
    private GameObject guildSearchCanvas;
    [SerializeField]
    private GameObject createButton;

    private string guildName;

    // Start is called before the first frame update
    void Start()
    {
        var textInput = textInputObject.GetComponent<TMP_InputField>();
        textInput.onValueChanged.AddListener(delegate
        {
            guildName = textInput.text;
        });
        createButton.GetComponent<Button>().onClick.AddListener(CreateGuild);
        
    }

    async void CreateGuild()
    {
        var guild = new GuildCreateModel(guildName, "", PlayerProfile.id);
        var createdGuild = await Requests.POSTGuild(guild);
        PlayerProfile.guildId = createdGuild.id;

        var guildScript = guildPageCanvas.GetComponent<GuildScript>();
        await guildScript.Load();

        var guildCreateCanvasGroup = GetComponent<CanvasGroup>();
        var guildSearchCanvasGroup = guildSearchCanvas.GetComponent<CanvasGroup>();
        var guildCanvasGroup = guildScript.GetComponent<CanvasGroup>();

        guildCreateCanvasGroup.alpha = 0;
        guildCreateCanvasGroup.interactable = false;
        guildCreateCanvasGroup.blocksRaycasts = false;

        guildSearchCanvasGroup.alpha = 0;

        guildCanvasGroup.alpha = 1;
        guildCanvasGroup.interactable = true;
        guildCanvasGroup.blocksRaycasts = true;
    }
}
