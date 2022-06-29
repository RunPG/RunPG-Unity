using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using Photon.Pun;
using System;
using UnityEngine.Networking;
using Photon.Realtime;

namespace RunPG.Multi
{
    public class ChatManager : MonoBehaviour, IChatClientListener,IOnEventCallback
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
        //FriendRequest display
        [SerializeField] GameObject friendrequestPrefab;
        [SerializeField] Transform friendrequestPrefabPos;
        //GuildDisplay display
        [SerializeField] GameObject guildNotificationPrefab;
        [SerializeField] Transform guildNotificationPrefabPos;
        //
        [SerializeField] Button addFriendButton;
        [SerializeField] InputField friendTextInput;
        public const byte AddedAsFriendEventCode = 1;

        private List<Notification> _notificationList = new List<Notification>();

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
            GetFriendRequestsAtStart();
            sendMsgButton.onClick.AddListener(SendMessage);
            addFriendButton.onClick.AddListener(delegate
            {
                OnclickAddFriend(friendTextInput.text);
            });
        }

        //Called at the start of the game and when a friend request is accepted, Instantiate the prefabs
        public void FriendlistPrefabInstantiation(string[] friends)
        {
            foreach (var friend in friends)
            {
                var display = Instantiate(friendlistPrefab, friendlistPrefabPos);
                display.transform.GetChild(0).GetComponent<Text>().text = friend;
            }
            _chatClient.AddFriends(friends);
        }

        public void GuildInvitationPrefabInstantiation(string guild, string sender_id)
        {
            var display = Instantiate(guildNotificationPrefab, guildNotificationPrefabPos);
            display.transform.GetChild(0).GetComponent<Text>().text = sender_id;
            display.transform.GetChild(1).GetComponent<Text>().text = guild;
        }
        //Sends a notification to the specified user
        public void OnclickAddFriend(string friend)
        {
            int? friend_id = Requests.GETPlayerID(friend, null);
            if (friend_id != null)
            {
                var coroutine = Requests.POSTSendNotification(Int32.Parse(PhotonNetwork.NickName),friend_id.Value, NotificationType.FRIENDLIST);
                StartCoroutine(coroutine);
                Debug.Log("Starting POSTREGISTER" + PhotonNetwork.NickName);
                //TODO: test raise event and RPC for notif sending when the 2 players are connected
                /*RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(AddedAsFriendEventCode, friend_id, raiseEventOptions, SendOptions.SendReliable);*/
            }
            else
            {
                Debug.Log("This user does not exist !");
                //TODO: notif error
            }
        }

        //Called at the start of the game, persistence
        void GetFriendRequestsAtStart()
        {
            var friendRequests = Requests.GETNotificationsByType(Int32.Parse(PhotonNetwork.NickName),NotificationType.FRIENDLIST);
            List<(String,int)> sendersUsername = new List<(String,int)>();
            foreach (var friendRequest in friendRequests)
            {
                var username = Requests.GETPlayerName(friendRequest.senderId, null);
                sendersUsername.Add((username, friendRequest.senderId));
            }
            FriendRequestPrefabInstantiation(sendersUsername);
        }
        void GetGuildInvitationAtStart()
        {
            var guildInvitations = Requests.GETNotificationsByType(Int32.Parse(PhotonNetwork.NickName), NotificationType.GUILD);
            foreach (var guildInvitation in guildInvitations)
            {
                var username = Requests.GETPlayerName(guildInvitation.senderId, null);
               
                GuildInvitationPrefabInstantiation("", username);
            }
        }
        bool IsNewNotification(Notification notification)
        {
            foreach (var notif in _notificationList)
            {
                if (notif.senderId == notification.senderId && notif.receiverId == notification.receiverId)
                    return false;
            }
            return true;
        }
        //Called at the start of the game, Instantiate the prefabs
        void FriendRequestPrefabInstantiation(List<(String, int)> senders)
        {
            foreach (var sender in senders)
            {
                var display = Instantiate(friendrequestPrefab, friendrequestPrefabPos);
                display.transform.GetChild(0).GetComponent<Text>().text = sender.Item1;
                display.GetComponent<FriendRequestDisplay>().sender_id = sender.Item2;
                display.GetComponent<FriendRequestDisplay>().chatManager = this;

            }
        }
        //Called at the start of the game, persistence
        void GetFriendsAtStart()
        {

            var friendlist =  Requests.GETAllFriends(Int32.Parse(PhotonNetwork.NickName));
            //list of friends usernames
            List<String> friends = new List<String>();

            foreach (var friend in friendlist)
            {
                var username = Requests.GETPlayerName(friend.friendId, null);
                friends.Add(username);
            }
            FriendlistPrefabInstantiation(friends.ToArray());
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

        //TODO test implem
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


        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            if (eventCode == AddedAsFriendEventCode)
            {
                object data = photonEvent.CustomData;

                int friend_id = (int) data;
                Debug.Log("TEST:" + friend_id);
                Debug.Log("TEST:" + photonEvent.Sender);
            }
        }
    }
}