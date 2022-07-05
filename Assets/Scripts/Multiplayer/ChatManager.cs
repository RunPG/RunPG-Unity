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
using System.Linq;

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
        //friendDisplay display 
        [SerializeField] GameObject friendDisplayPrefab;
        [SerializeField] Transform friendDisplayPrefabPos;
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
        }

        //Called at the start of the game and when a friend request is accepted, Instantiate the prefabs
        public void friendDisplayPrefabInstantiation(List<(String, int)> friends)
        {
            foreach (var friend in friends)
            {
                var display = Instantiate(friendDisplayPrefab, friendDisplayPrefabPos);
                display.transform.GetChild(0).GetComponent<Text>().text = friend.Item1;
                display.GetComponent<FriendDisplay>()._friendId = friend.Item2;
            }
            _chatClient.AddFriends(friends.Select(friend => friend.Item1).ToArray());
        }
  
        //Called at the start of the game, persistence
        void GetFriendsAtStart()
        {

            var friendDisplay =  Requests.GETAllFriends(GlobalVariables.userId);
            //list of friends usernames
            List<(String,int)> friends= new List<(String, int)>();

            foreach (var friend in friendDisplay)
            {
                var username = Requests.GETPlayerName(friend.friendId, null);
            
                friends.Add((username,friend.friendId));
            }
            friendDisplayPrefabInstantiation(friends);
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
        }
    }
}