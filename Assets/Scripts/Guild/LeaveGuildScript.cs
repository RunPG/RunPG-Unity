using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaveGuildScript : MonoBehaviour
{
    [SerializeField]
    private GameObject guildPageCanvas;
    [SerializeField]
    private GameObject guildSearchCanvas;
    [SerializeField]
    private GameObject yesButtonObject;
    // Start is called before the first frame update
    void Start()
    {
        yesButtonObject.GetComponent<Button>().onClick.AddListener(LeaveGuild);
    }

    async void LeaveGuild()
    {
        var guild = guildPageCanvas.GetComponent<GuildScript>().guild; 
        guild.members = guild.members.Where(user => user.id != PlayerProfile.id).ToList();
        await Requests.DELETELeaveGuild(PlayerProfile.id);
        PlayerProfile.guildId = 0;
        PlayerProfile.isGuildOwner = false;

        var guildPageCanvasGroup = guildPageCanvas.GetComponent<CanvasGroup>();
        guildPageCanvasGroup.alpha = 0;
        guildPageCanvasGroup.interactable = false;
        guildPageCanvasGroup.blocksRaycasts = false;

        await guildSearchCanvas.GetComponent<GuildSearchScript>().Load();
        var guildSearchCanvasGroup = guildSearchCanvas.GetComponent<CanvasGroup>();
        guildSearchCanvasGroup.alpha = 1;
        guildSearchCanvasGroup.interactable = true;
        guildSearchCanvasGroup.blocksRaycasts = true;
        guildSearchCanvas.SetActive(true);
    }
}
