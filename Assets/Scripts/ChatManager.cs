using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using Photon.Pun;
using System;
using Photon.Realtime;
using UnityEngine.Networking;
using System.Text;
using Assets.Scripts;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient _chatClient;
    [SerializeField] InputField msgTextInput;
    [SerializeField] Button sendMsgButton;
    //Messages display
    [SerializeField] GameObject msgPrefab;
    [SerializeField] Transform msgPrefabPos;
    //Friendlist display 
    [SerializeField] GameObject friendlistPrefab;
    [SerializeField] Transform friendlistPrefabPos;

    [SerializeField] Button addFriendButton;
    [SerializeField] InputField friendTextInput;

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange:OnConnected called");

    }
    void Start()
    {
        GetFriendsAtStart();
        sendMsgButton.onClick.AddListener(SendMessage);
        addFriendButton.onClick.AddListener(delegate
        {
            OnclickAddFriend(friendTextInput.text);
        });
    }
    public void AddFriends(string[] friends)
    {
        Debug.Log("AddFriend");
        foreach (var friend in friends)
        {
            Debug.Log("Adding ");
            var display = Instantiate(friendlistPrefab, friendlistPrefabPos);
            display.transform.GetChild(0).GetComponent<Text>().text = friend;
        }
        _chatClient.AddFriends(friends);
    }
    public void OnclickAddFriend(string friend)
    {
        int? friend_id = Requests.GETPlayerID(friend, null);
        if (friend_id != null)
        {
            var coroutine = Requests.POSTAddFriend(friend_id.Value);
            StartCoroutine(coroutine);
            Debug.Log("Starting POSTREGISTER" + PhotonNetwork.NickName);
            //TODO GET USER ID IN THE DB AND THEN SEND RPC TO THIS USER ID
            //  Player player = null;
            //   PhotonView photonView = PhotonView.Get(this);
            //   photonView.RPC("AddFriendAskConfirmation", player);
        }
        else
        {
            Debug.Log("This user does not exist !");
            //TODO: notif error
        }
        string[] friends = new string[1];
        friends[0] = friend;
        if (_chatClient.AddFriends(friends))
        {
            var display = Instantiate(friendlistPrefab, friendlistPrefabPos);
            display.transform.GetChild(0).GetComponent<Text>().text = friend;
        }
    }
    void GetFriendsAtStart()
    {

        var friendlist = Requests.GETAllFriends(Int32.Parse(PhotonNetwork.NickName));
        List<String> friends = new List<String>();

        foreach (var friend in friendlist)
        {
            Debug.Log("friend:" + friend.friendId);
           var username = Requests.GETPlayerName(friend.friendId,null);
           friends.Add(username);
        }
        AddFriends(friends.ToArray());
    }
    bool GETPlayer(String username)
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://178.62.237.73/user/" + friendTextInput.text))
        {
            request.SendWebRequest();
            while (!request.isDone)
            {
                //TODO change 
                //waiting for request to be done
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.result + ":" + username);
                return false;
            }
            else
            {
                Debug.Log(request.result);
                return true;
            }
        }
    }
    [PunRPC]
    public void AddFriendAskConfirmation(string friend)
    {
        Debug.Log("Added");
    }
    public void OnConnected()   
    {
        Debug.Log("ChatManager:OnConnected called");
        _chatClient.Subscribe("guild", creationOptions: new ChannelCreationOptions { MaxSubscribers = 50 });
        _chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnDisconnected()
    {
        Debug.Log("ChatManager:OnDisconnected called");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        Debug.Log("OnGetMessages");
        
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            msgs = string.Format("{0}{1}={2}, ", msgs, senders[i], messages[i]);
            if (senders[i] != PhotonNetwork.NickName)
                DisplayMessage(messages[i].ToString(), senders[i]);

        }
        // All public messages are automatically cached in `Dictionary<string, ChatChannel> PublicChannels`.
        // So you don't have to keep track of them.
        // The channel name is the key for `PublicChannels`.
        // In very long or active conversations, you might want to trim each channels history.
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("ChatManager:OnStatusUpdate called: " + user + status);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log("ChatManager: OnUserSubscribed called");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
    private void Awake()
    {
        _chatClient = new ChatClient(this);
        _chatClient.ChatRegion = "EU";

        Debug.Log(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat);
        _chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
        Application.runInBackground = true;
    }

    // Start is called before the first frame update

    void SendMessage()
    {
        DisplayMessage(msgTextInput.text, PhotonNetwork.NickName);
        _chatClient.PublishMessage("guild", msgTextInput.text);
        msgTextInput.text = "";

    }
    public void DisplayMessage(string message, string sender)
    {
        var msg = Instantiate(msgPrefab, msgPrefabPos);
        msg.transform.GetChild(0).GetComponent<Text>().text = message;
        msg.transform.GetChild(1).GetComponent<Text>().text = sender;
    }
    // Update is called once per frame
    void Update()
    {
        _chatClient.Service();
    }
    
}
