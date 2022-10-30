using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSelectionScript : MonoBehaviour
{
    [SerializeField]
    private GameObject menuSelectionButton;
    
    [SerializeField]
    private GameObject guildSearchCanvas;
    [SerializeField]
    private GameObject guildPageCanvas;
    [SerializeField]
    private GameObject guildButton;

    [SerializeField]
    private GameObject socialCanvas;
    [SerializeField]
    private GameObject socialButton;



    [SerializeField]
    private GameObject notificationManagerCanvas;

    // Start is called before the first frame update
    void Start()
    {
        var notificationManagerScript = notificationManagerCanvas.GetComponent<NotificationManagerScript>();

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
                notificationManagerScript.guildMessagesNotification = false;
                notificationManagerScript.UpdateNotificationObjects();
            }
        });

        socialButton.GetComponent<Button>().onClick.AddListener(async () =>
        {
            var socialScript = socialCanvas.GetComponent<SocialScript>();
            await socialScript.Load();
            var socialCanvasGroup = socialCanvas.GetComponent<CanvasGroup>();
            socialCanvasGroup.alpha = 1;
            socialCanvasGroup.interactable = true;
            socialCanvasGroup.blocksRaycasts = true;
        });
    }
}
