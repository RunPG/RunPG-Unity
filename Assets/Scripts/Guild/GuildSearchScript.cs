using Assets.Scripts.Multiplayer.Request_Models;
using Mapbox.Unity.Utilities;
using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildSearchScript : MonoBehaviour
{
    [SerializeField]
    private GameObject guildLayout;
    [SerializeField]
    private GameObject guildPrefab;
    [SerializeField]
    private GameObject guildInvitationPrefab;
    [SerializeField]
    private GameObject textInputObject;
    [SerializeField]
    private GameObject guildsTextObject;
    [SerializeField]
    private GameObject invitationTextObject;
    [SerializeField]
    private GameObject guildPageCanvas;

    private string filter;
    
    private List<GuildModel> guilds;
    private List<GuildModel> filteredGuilds;

    private List<GuildModel> guildInvitations;
    private List<GuildModel> filteredGuildInvitations;

    void Start()
    {
        var textInput = textInputObject.GetComponent<TMP_InputField>();
        textInput.onValueChanged.AddListener(delegate
        {
            FilterList(textInput.text);
        });
        filter = textInput.text;
    }

    async Task LoadGuilds()
    {
        guilds = new List<GuildModel>();
        var guildModels = await Requests.GETAllGuilds();
        foreach (var guildModel in guildModels)
        {
            //FIXME (attendre modifications back)
            var tmp = await Requests.GETGuildById(guildModel.id);
            guilds.Add(tmp);
        }
    }

    async Task LoadInvitations()
    {
        guildInvitations = new List<GuildModel>();
        var notifications = await Requests.GETNotificationsByType(PlayerProfile.id, NotificationType.GUILD);
        foreach (var notification in notifications)
        {
            var user = await Requests.GETUserById(notification.senderId);
            try
            {
                var guild = await Requests.GETGuildById(user.guildId);
                guildInvitations.Add(guild);
            }
            catch
            {
                Debug.Log("Guild not found: " + user.guildId);
            }
        }
    }

    void ClearGuildList()
    {
        foreach (Transform child in guildLayout.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void AddGuilds()
    {
        if (filteredGuilds.Count > 0)
        {
            Instantiate(guildsTextObject, guildLayout.transform);
        }
        foreach (var guild in filteredGuilds)
        {
            var guildObject = Instantiate(guildPrefab, guildLayout.transform).transform;
            guildObject.Find("Name").GetComponent<TextMeshProUGUI>().text = guild.name;
            guildObject.Find("Capacity").GetComponent<TextMeshProUGUI>().text = guild.members.Count + "/" + 50;
            guildObject.Find("Join").GetComponent<Button>().onClick.AddListener(() =>
            {
                JoinGuild(guild);
                guildObject.Find("Join").GetComponent<Image>().sprite = Resources.Load<Sprite>("Social/Accept");
            });
        }
    }

    void AddInvitations()
    {
        if (filteredGuildInvitations.Count > 0)
        {
            Instantiate(invitationTextObject, guildLayout.transform);
        }
        foreach (var guild in filteredGuildInvitations)
        {
            var guildObject = Instantiate(guildInvitationPrefab, guildLayout.transform).transform;
            guildObject.Find("Name").GetComponent<TextMeshProUGUI>().text = guild.name;
            guildObject.Find("Capacity").GetComponent<TextMeshProUGUI>().text = guild.members.Count + "/" + 50;
            guildObject.Find("Accept").GetComponent<Button>().onClick.AddListener(() =>
            {
                AcceptInvitation(guild);
            });
            guildObject.Find("Decline").GetComponent<Button>().onClick.AddListener(() =>
            {
                DeclineInvitation(guild);
            });
        }
    }

    async void AcceptInvitation(GuildModel guild)
    {
        var guildOwner = guild.members.First(m => m.isOwner);
        await Requests.DELETENotification(PlayerProfile.id, guildOwner.id, NotificationType.GUILD);
        await Requests.POSTJoinGuild(PlayerProfile.id, guild.id);
        PlayerProfile.guildId = guild.id;


        var guildScript = guildPageCanvas.GetComponent<GuildScript>();
        await guildScript.Load();

        var guildSearchCanvasGroup = GetComponent<CanvasGroup>();
        guildSearchCanvasGroup.alpha = 0;
        guildSearchCanvasGroup.interactable = false;
        guildSearchCanvasGroup.blocksRaycasts = false;

        var guildCanvasGroup = guildScript.GetComponent<CanvasGroup>();
        guildCanvasGroup.alpha = 1;
        guildCanvasGroup.interactable = true;
        guildCanvasGroup.blocksRaycasts = true;
    }

    async void DeclineInvitation(GuildModel guild)
    {
        var guildOwner = guild.members.Find(user => user.isOwner);
        await Requests.DELETENotification(PlayerProfile.id, guildOwner.id, NotificationType.GUILD);
        guildInvitations.Remove(guild);
        ReloadGuildList();
    }

    async void JoinGuild(GuildModel guild)
    {
        var guildOwner = guild.members.Find(user => user.isOwner);
        await Requests.POSTSendNotification(PlayerProfile.id, guildOwner.id, NotificationType.GUILD);
    }

    void FilterList()
    {
        filteredGuilds = guilds.Where(guild => guild.name.ToLower().Contains(filter.ToLower()) && !guildInvitations.Contains(guild)).ToList();
        filteredGuildInvitations = guildInvitations.Where(guild => guild.name.ToLower().Contains(filter.ToLower())).ToList();
    }

    void FilterList(string value)
    {
        filter = value;
        ReloadGuildList();
    }

    public async Task Load()
    {
        await LoadGuilds();
        await LoadInvitations();
        ReloadGuildList();
    }

    void ReloadGuildList()
    {
        FilterList();
        ClearGuildList();
        AddInvitations();
        AddGuilds();
    }
}
