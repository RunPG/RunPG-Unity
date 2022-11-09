using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RunPG.Multi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPLayerListScript : MonoBehaviour
{
    [SerializeField]
    private GameObject playerListLayout;
    [SerializeField]
    private GameObject guildPrefab;
    [SerializeField]
    private GameObject friendPrefab;
    [SerializeField]
    private GameObject guildTextObject;
    [SerializeField]
    private GameObject friendsTextObject;
    [SerializeField]
    private TMP_InputField textInput;
    [SerializeField]
    private ChatManagerScript chatManagerScript;

    private string roomName;

    private string filter;

    private List<UserInfos> friends;
    private GuildModel guild;

    private List<UserInfos> filteredFriends;
    private bool includeGuild;

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
        foreach (Transform child in playerListLayout.transform)
        {
            Destroy(child.gameObject);
        }
    }

    async Task LoadFriends()
    {
        friends = new List<UserInfos>();
        
        FriendlistModel friendListModel = await Requests.GETAllFriends(PlayerProfile.id);
        foreach (var friendModel in friendListModel.friends)
        {
            UserModel userModel = await Requests.GETUserById(friendModel.friendId);
            UserCharacterModel characterModel = await Requests.GETUserCharacter(friendModel.friendId);
            friends.Add(new UserInfos(userModel, characterModel));
        }
    }

    async Task LoadGuild()
    {
        if (PlayerProfile.guildId.HasValue)
        {
            guild = await Requests.GETGuildById(PlayerProfile.guildId.Value);
            includeGuild = true;
        }
        else
        {
            includeGuild = false;
        }
    }

    void AddGuild()
    {
        if (!includeGuild)
            return;
        Instantiate(guildTextObject, playerListLayout.transform);
        Transform guildObject = Instantiate(guildPrefab, playerListLayout.transform).transform;
        guildObject.Find("Name").GetComponent<TextMeshProUGUI>().text = guild.name;
        guildObject.Find("Capacity").GetComponent<TextMeshProUGUI>().text = guild.members.Count + "/" + 50;
        guildObject.Find("Invite").GetComponent<Button>().onClick.AddListener(() => {
            InviteGuild();
            guildObject.Find("Invite").GetComponent<Image>().sprite = Resources.Load<Sprite>("Social/Check");
        });

    }

    void AddFriends()
    {
        if (filteredFriends.Count > 0)
        {
            Instantiate(friendsTextObject, playerListLayout.transform);
        }
        foreach(var friend in filteredFriends)
        {
            Transform friendObject = Instantiate(friendPrefab, playerListLayout.transform).transform;
            friendObject.Find("Class").GetComponent<Image>().sprite = friend.heroClass.GetSprite();
            friendObject.Find("Name").GetComponent<TextMeshProUGUI>().text = friend.name;
            friendObject.Find("Level").GetComponent<TextMeshProUGUI>().text = string.Format("Lv. {0}", friend.level);
            friendObject.Find("Invite").GetComponent<Button>().onClick.AddListener(() => {
                InviteFriend(friend);
                friendObject.Find("Invite").GetComponent<Image>().sprite = Resources.Load<Sprite>("Social/Check");
            });
        }
    }

    void InviteGuild()
    {
        chatManagerScript.SendGuildLobbyInvitation(roomName);
    }
    void InviteFriend(UserInfos friend)
    {
        chatManagerScript.SendFriendLobbyInvitation(friend.name, roomName);
    }

    void FilterList()
    {
        filteredFriends = friends.Where(friend => friend.name.ToLower().Contains(filter.ToLower())).ToList();
        if (guild != null)
            includeGuild = guild.name.ToLower().Contains(filter.ToLower());
    }

    async void FilterList(string value)
    {
        filter = value;
        await ReloadList();
    }

    async Task ReloadList()
    {
        await LoadGuild();
        await LoadFriends();
        FilterList();
        ClearList();
        AddGuild();
        AddFriends();
    }

    public async Task LoasList(string roomName)
    {
        this.roomName = roomName;
        await ReloadList();
    }
}