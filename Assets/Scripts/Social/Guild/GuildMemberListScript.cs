using Assets.Scripts.Multiplayer.Request_Models;
using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildMemberListScript : MonoBehaviour
{
    [SerializeField]
    private GuildScript guildScript;
    [SerializeField]
    private GameObject userListLayout;
    [SerializeField]
    private GameObject requestUserPrefab;
    [SerializeField]
    private GameObject userPrefab;
    [SerializeField]
    private GameObject requestTextObject;
    [SerializeField]
    private GameObject membersTextObject;
    [SerializeField]
    private TMP_InputField textInput;
    [SerializeField]
    private NotificationManagerScript notificationManagerScript;

    private string filter;

    private List<GuildMemberModel> requestUsers;

    private List<GuildMemberModel> filteredMembers;
    private List<GuildMemberModel> filteredRequests;

    void Start()
    {
        textInput.onValueChanged.AddListener(delegate
        {
            FilterList(textInput.text);
        });
        filter = textInput.text;
    }

    void ClearList()
    {
        foreach (Transform child in userListLayout.transform)
        {
            Destroy(child.gameObject);
        }
    }
    async Task LoadJoinRequests()
    {
        requestUsers = new List<GuildMemberModel>();
        var notifications = await Requests.GETNotificationsByType(guildScript.guildOwner.id, NotificationType.GUILD);
        foreach (var notification in notifications)
        {
            var user = await Requests.GETUserById(notification.senderId);
            var character = await Requests.GETUserCharacter(user.id);
            var member = new GuildMemberModel(user.id, user.name, false, character.character.heroClass, character.statistics.level);
            requestUsers.Add(member);
        }
    }

    void AddMembers()
    {
        if (filteredMembers.Count > 0)
        {
            Instantiate(membersTextObject, userListLayout.transform);
        }
        foreach(var member in filteredMembers)
        {
            Transform userObject = Instantiate(userPrefab, userListLayout.transform).transform;
            userObject.Find("Class").GetComponent<Image>().sprite = member.heroClass.GetSprite();
            userObject.Find("Name").GetComponent<TextMeshProUGUI>().text = member.name;
            userObject.Find("Level").GetComponent<TextMeshProUGUI>().text = string.Format("Lv. {0}", member.level);

            if (guildScript.guildOwner.id == PlayerProfile.id)
                userObject.Find("RemoveButton").GetComponent<Button>().onClick.AddListener(() => RemoveMember(member));
            else
                Destroy(userObject.Find("RemoveButton").gameObject);
        }
    }
    
    async void RemoveMember(GuildMemberModel member)
    {
        guildScript.guild.members = guildScript.guild.members.Where(user => user.id != PlayerProfile.id).ToList();
        await Requests.DELETELeaveGuild(member.id);
        ReloadUsersList();
    }

    void AddRequests()
    {
        if (filteredRequests.Count > 0)
        {
            Instantiate(requestTextObject, userListLayout.transform);
        }
        foreach (var user in filteredRequests)
        {
            Transform userObject = Instantiate(requestUserPrefab, userListLayout.transform).transform;
            userObject.Find("Class").GetComponent<Image>().sprite = user.heroClass.GetSprite();
            userObject.Find("Name").GetComponent<TextMeshProUGUI>().text = user.name;
            userObject.Find("Level").GetComponent<TextMeshProUGUI>().text = string.Format("Lv. {0}", user.level);
            userObject.Find("AcceptButton").GetComponent<Button>().onClick.AddListener(() => AcceptJoinRequest(user));
            userObject.Find("DeclineButton").GetComponent<Button>().onClick.AddListener(() => DeclineJoinRequest(user));
        }
    }

    async void AcceptJoinRequest(GuildMemberModel member)
    {
        await Requests.DELETENotification(PlayerProfile.id, member.id, NotificationType.GUILD);
        await Requests.POSTJoinGuild(member.id, guildScript.guild.id);
        guildScript.guild.members.Add(member);
        requestUsers.Remove(member);
        if (requestUsers.Count == 0)
        {
            await notificationManagerScript.UpdateNotifications();
        }
        ReloadUsersList();
    }

    async void DeclineJoinRequest(GuildMemberModel member)
    {
        await Requests.DELETENotification(PlayerProfile.id, member.id, NotificationType.GUILD);
        requestUsers.Remove(member);
        if (requestUsers.Count == 0)
        {
            await notificationManagerScript.UpdateNotifications();
        }
        ReloadUsersList();
    }

    void FilterList()
    {
        filteredMembers = guildScript.guild.members.Where(member => member.name.ToLower().Contains(filter.ToLower())).ToList();
        filteredRequests = requestUsers.Where(member => member.name.ToLower().Contains(filter.ToLower())).ToList();
    }

    void FilterList(string value)
    {
        filter = value;
        ReloadUsersList();
    }

    public async void ReloadUsersList()
    {
        Debug.Log("ReloadUsersList");
        await LoadJoinRequests();
        FilterList();
        ClearList();
        if (guildScript.guildOwner.id == PlayerProfile.id)
            AddRequests();
        AddMembers();
    }
}
