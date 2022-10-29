using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
using System.Threading.Tasks;
using Unity.Services.Authentication;

namespace RunPG.Multi
{
    public class ChatManagerScript : MonoBehaviour, IChatClientListener, IOnEventCallback
    {
        [SerializeField]
        private GameObject guildPageCanvas;
        [SerializeField]
        private GameObject friendChatCanvas;

        private string guildChannelName;
        private ChatClient _chatClient;
        
        private GuildScript guildScript;
        private FriendChatScript friendChatScript;

        private void Awake()
        {
            _chatClient = new ChatClient(this);
            _chatClient.ChatRegion = "EU";

            Debug.Log(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat);
            _chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PlayerProfile.pseudo));
            Application.runInBackground = true;
        }

        private void Start()
        {
            guildScript = guildPageCanvas.GetComponent<GuildScript>();
            friendChatScript = friendChatCanvas.GetComponent<FriendChatScript>();
            guildChannelName = $"Guild_{PlayerProfile.guildId}";
        }
        

        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.Log(message);
        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log("OnChatStateChange:OnConnected called");
        }
        
        public void OnConnected()
        {
            Debug.Log("ChatManager:OnConnected called");
            if (PlayerProfile.guildId != null && PlayerProfile.guildId != 0)
            {
                if (_chatClient.Subscribe(guildChannelName, messagesFromHistory: 50, creationOptions: new ChannelCreationOptions { MaxSubscribers = 50 }))
                {
                    _chatClient.PublicChannels[guildChannelName].Messages.ForEach(messageObject =>
                    {
                        var messageArray = (string[])messageObject;
                        var sender = messageArray[1];
                        var message = messageArray[1];
                        guildScript.DisplayMessage(message, sender);
                    });
                    Debug.Log($"Subscribed to {guildChannelName}");
                }
            }
            //_chatClient.SetOnlineStatus(ChatUserStatus.Online);
        }

        public void OnDisconnected()
        {
            Debug.Log("ChatManager:OnDisconnected called");
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            for (int i = 0; i < messages.Length; i++)
            {
                string[] message = (string[])messages[i];
                if (message != null)
                    guildScript.DisplayMessage(message[0], message[1]);
            }
            // All public messages are automatically cached in `Dictionary<string, ChatChannel> PublicChannels`.
            // So you don't have to keep track of them.
            // The channel name is the key for `PublicChannels`.
            // In very long or active conversations, you might want to trim each channels history.
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            Debug.Log($"OnPrivateMessage: {sender} sent {message} to {channelName}");
            var messageArray = (string[])message;
            var messageText = messageArray[0];
            var messageSender = messageArray[1];
            if (friendChatScript.currentFriend != null && sender == friendChatScript.currentFriend.name)
            {
                friendChatScript.AddMessage(messageText, messageSender);
            }
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

        // Start is called before the first frame update

        public void SendGuildMessage(string messageText)
        {
            string[] message = { messageText, PlayerProfile.pseudo };
            _chatClient.PublishMessage(guildChannelName, message);
        }

        public void SendPrivateMessage(string target, string messageText)
        {
            string[] message = { messageText, PlayerProfile.pseudo };
            _chatClient.SendPrivateMessage(target, message);
        }

        public List<string[]> GetPrivateMessages(string target)
        {
            string privateChannelName = _chatClient.GetPrivateChannelNameByUser(target);

            if (_chatClient.TryGetChannel(privateChannelName, true, out ChatChannel privateChannel))
            {
                List<string[]> messages = privateChannel.Messages.Select(messageObject =>
                {
                    var messageArray = (string[])messageObject;
                    return new string[] { messageArray[0], messageArray[1] };
                }).ToList();
                return messages;
            }
            return new List<string[]>();
        }

        private void Update()
        {
            if (_chatClient != null)
            {
                _chatClient.Service();
            }
        }

        public void OnEvent(EventData photonEvent)
        {
        }
    }
}