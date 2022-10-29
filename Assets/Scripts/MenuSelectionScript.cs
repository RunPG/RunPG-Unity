using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSelectionScript : MonoBehaviour
{
    [SerializeField]
    private GameObject guildSearchCanvas;
    [SerializeField]
    private GameObject guildPageCanvas;
    [SerializeField]
    private GameObject guildButton;
    [SerializeField]
    private GameObject menuSelectionButton;

    // Start is called before the first frame update
    void Start()
    {
        menuSelectionButton.GetComponent<Button>().onClick.AddListener(async () =>
        {
            var user = await Requests.GETUserById(PlayerProfile.id);
            if (user != null)
            {
                PlayerProfile.characterInfo = await CharacterInfo.Load(PlayerProfile.id);
                PlayerProfile.guildId = user.guildId;
                PlayerProfile.isGuildOwner = user.isGuildOwner;
            }
        });

        guildButton.GetComponent<Button>().onClick.AddListener(async () =>
        {
            
            if (PlayerProfile.guildId == null || PlayerProfile.guildId == 0)
            {
                var guildSearchScript = guildSearchCanvas.GetComponent<GuildSearchScript>();
                await guildSearchScript.Load();
                var guildSearchCanvasGroup = guildSearchCanvas.GetComponent<CanvasGroup>();
                guildSearchCanvasGroup.alpha = 1;
                guildSearchCanvasGroup.interactable = true;
                guildSearchCanvasGroup.blocksRaycasts = true;
            }
            else
            {
                var guildScript = guildPageCanvas.GetComponent<GuildScript>();
                await guildScript.Load();
                var guildPageCanvasGroup = guildPageCanvas.GetComponent<CanvasGroup>();
                guildPageCanvasGroup.alpha = 1;
                guildPageCanvasGroup.interactable = true;
                guildPageCanvasGroup.blocksRaycasts = true;
            }
        });
    }
}
