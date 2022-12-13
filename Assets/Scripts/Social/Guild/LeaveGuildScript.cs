using RunPG.Multi;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaveGuildScript : MonoBehaviour
{
    [SerializeField]
    private GuildScript guildScript;
    [SerializeField]
    private CanvasGroup guildCanvasGroup;
    [SerializeField]
    private GuildSearchScript guildSearchScript;
    [SerializeField]
    private CanvasGroup guildSearchCanvasGroup;
    [SerializeField]
    private GameObject yesButtonObject;

    void Start()
    {
        yesButtonObject.GetComponent<Button>().onClick.AddListener(LeaveGuild);
    }

    async void LeaveGuild()
    {
        var guild = guildScript.guild; 
        guild.members = guild.members.Where(user => user.id != PlayerProfile.id).ToList();
        await Requests.DELETELeaveGuild(PlayerProfile.id);
        await NotificationManagerScript.instance.ClearGuildNotification();
        PlayerProfile.guildId = 0;
        PlayerProfile.isGuildOwner = false;

        guildCanvasGroup.alpha = 0;
        guildCanvasGroup.interactable = false;
        guildCanvasGroup.blocksRaycasts = false;

        await guildSearchScript.Load();
        guildSearchCanvasGroup.alpha = 1;
        guildSearchCanvasGroup.interactable = true;
        guildSearchCanvasGroup.blocksRaycasts = true;
    }
}
