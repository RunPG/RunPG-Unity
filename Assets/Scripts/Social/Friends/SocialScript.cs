using RunPG.Multi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SocialScript : MonoBehaviour
{
    [SerializeField]
    private GameObject friendListLayout;
    [SerializeField]
    private GameObject friendPrefab;
    [SerializeField]
    private GameObject friendRequestPrefab;
    [SerializeField]
    private GameObject friendRequestText;
    [SerializeField]
    private GameObject myFriendsText;
    [SerializeField]
    private TMP_Dropdown dropdown;
    [SerializeField]
    private TMP_InputField textInput;
    [SerializeField]
    private CanvasGroup friendChatCanvasGroup;
    [SerializeField]
    private FriendChatScript friendChatScript;
    [SerializeField]
    private NotificationManagerScript notificationManagerScript;


    public List<UserInfos> friends;
    private List<UserInfos> filteredFriends;

    public List<UserInfos> friendRequests;
    private List<UserInfos> filteredFriendRequests;

    private Order order;
    private bool ascendant;

    private string filter;

    // Start is called before the first frame update
    void Start()
    {

        dropdown.onValueChanged.AddListener(delegate
        {
            ReOrderList(dropdown.value);
        });

        textInput.onValueChanged.AddListener(delegate
        {
            FilterList(textInput.text);
        });
        filter = textInput.text;

        order = Order.NAME;
        ascendant = true;
    }

    async Task LoadFriendRequests()
    {
        friendRequests = new List<UserInfos>();
        
        NotificationModel[] notificationModels = await Requests.GETNotificationsByType(PlayerProfile.id, NotificationType.FRIENDLIST);
        foreach (var notification in notificationModels)
        {
            UserModel userModel = await Requests.GETUserById(notification.senderId);
            UserCharacterModel characterModel = await Requests.GETUserCharacter(notification.senderId);
            friendRequests.Add(new UserInfos(userModel, characterModel));
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

    public async Task Load()
    {
        await LoadFriends();
        await LoadFriendRequests();
        ReloadFriendList();
    }

    void ClearFriendlist()
    {
        foreach (Transform child in friendListLayout.transform)
        {
             Destroy(child.gameObject);
        }
    }

    void AddFriends()
    {
        if (filteredFriends.Count != 0)
        {
            Instantiate(myFriendsText, friendListLayout.transform);
        }
        foreach (var friend in filteredFriends)
        {
            Transform friendObject = Instantiate(friendPrefab, friendListLayout.transform).transform;
            friendObject.Find("Class").GetComponent<Image>().sprite = friend.heroClass.GetSprite();
            friendObject.Find("Name").GetComponent<TextMeshProUGUI>().text = friend.name;
            friendObject.Find("Level").GetComponent<TextMeshProUGUI>().text = string.Format("Lv. {0}", friend.level);
            
            var messageTransform = friendObject.Find("Message");
            var notificationObject = messageTransform.Find("Notification").gameObject;

            if (notificationManagerScript.friendMessagesSenders.Contains(friend.name))
            {
                notificationObject.SetActive(true);
            }

            messageTransform.GetComponent<Button>().onClick.AddListener(() =>
            {
                notificationObject.SetActive(false);
                notificationManagerScript.friendMessagesSenders.Remove(friend.name);

                friendChatScript.Connect(friend);
                friendChatCanvasGroup.alpha = 1;
                friendChatCanvasGroup.blocksRaycasts = true;
                friendChatCanvasGroup.interactable = true;

                var socialGroup = GetComponent<CanvasGroup>();
                socialGroup.alpha = 0;
                socialGroup.blocksRaycasts = false;
                socialGroup.interactable = false;
            });
            //TODO Messages
        }
    }

    void AddFriendRequests()
    {
        if (filteredFriendRequests.Count != 0)
        {
            Instantiate(friendRequestText, friendListLayout.transform);
        }
        foreach (var friend in filteredFriendRequests)
        {
            Transform friendObject = Instantiate(friendRequestPrefab, friendListLayout.transform).transform;
            friendObject.Find("Class").GetComponent<Image>().sprite = friend.heroClass.GetSprite();
            friendObject.Find("Name").GetComponent<TextMeshProUGUI>().text = friend.name;
            friendObject.Find("Level").GetComponent<TextMeshProUGUI>().text = string.Format("Lv. {0}", friend.level);
            friendObject.Find("Accept").GetComponent<Button>().onClick.AddListener(() => AcceptFriendRequest(friend));
            friendObject.Find("Decline").GetComponent<Button>().onClick.AddListener(() => DeclineFriendRequest(friend));
        }
    }

    async void AcceptFriendRequest(UserInfos friend)
    {
        await Requests.DELETENotification(PlayerProfile.id, friend.id, NotificationType.FRIENDLIST);
        await Requests.POSTAddFriend(PlayerProfile.id, friend.id);
        await Requests.POSTAddFriend(friend.id, PlayerProfile.id);
        friendRequests.Remove(friend);
        if (friendRequests.Count == 0)
        {
            await notificationManagerScript.UpdateNotifications();
        }
        friends.Add(friend);
        ReloadFriendList();
    }

    async void DeclineFriendRequest(UserInfos friend)
    {
        await Requests.DELETENotification(PlayerProfile.id, friend.id, NotificationType.FRIENDLIST);
        friendRequests.Remove(friend);
        if (friendRequests.Count == 0)
        {
            await notificationManagerScript.UpdateNotifications();
        }
        ReloadFriendList();
    }

    void ReOrderList(int val)
    {
        switch (val)
        {
            case 0:
                order = Order.NAME;
                ascendant = true;
                break;
            case 1:
                order = Order.NAME;
                ascendant = false;
                break;
            case 2:
                order = Order.LEVEL;
                ascendant = true;
                break;
            case 3:
                order = Order.LEVEL;
                ascendant = false;
                break;
        }
        ReloadFriendList();
    }

    void OrderList()
    {
        switch (order, ascendant)
        {
            case (Order.NAME, true):
                filteredFriends.Sort((x, y) => x.name.CompareTo(y.name));
                filteredFriendRequests.Sort((x, y) => x.name.CompareTo(y.name));
                break;
            case (Order.NAME, false):
                filteredFriends.Sort((x, y) => y.name.CompareTo(x.name));
                filteredFriendRequests.Sort((x, y) => y.name.CompareTo(x.name));
                break;
            case (Order.LEVEL, true):
                filteredFriends.Sort((x, y) => x.level.CompareTo(y.level));
                filteredFriendRequests.Sort((x, y) => x.level.CompareTo(y.level));
                break;
            case (Order.LEVEL, false):
                filteredFriends.Sort((x, y) => y.level.CompareTo(x.level));
                filteredFriendRequests.Sort((x, y) => y.level.CompareTo(x.level));
                break;
        }
    }

    void FilterList()
    {
        filteredFriends = friends.Where(friend => friend.name.ToLower().Contains(filter.ToLower())).ToList();
        filteredFriendRequests = friendRequests.Where(friend => friend.name.ToLower().Contains(filter.ToLower())).ToList();
    }

    void FilterList(string value)
    {
        filter = value;
        ReloadFriendList();
    }

    void ReloadFriendList()
    {
        FilterList();
        OrderList();
        ClearFriendlist();
        AddFriendRequests();
        AddFriends();
    }

    enum Order
    {
        NAME,
        LEVEL
    }
}