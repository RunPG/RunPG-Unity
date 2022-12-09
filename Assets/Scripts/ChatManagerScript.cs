using System.Collections.Generic;
using UnityEngine;

using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

namespace RunPG.Multi
{
    public class ChatManagerScript : MonoBehaviour, IChatClientListener, IOnEventCallback
    {
        [SerializeField]
        private GameObject guildPageCanvas;
        [SerializeField]
        private GameObject friendChatCanvas;
        [SerializeField]
        private GameObject notificationManagerCanvas;

        private string guildChannelName;
        private ChatClient _chatClient;
        
        private GuildScript guildScript;
        private CanvasGroup guildGroup;
        
        private FriendChatScript friendChatScript;
        private CanvasGroup friendChatGroup;

        private NotificationManagerScript notificationManagerScript;
        public static ChatManagerScript instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            _chatClient = new ChatClient(this);
            _chatClient.ChatRegion = "EU";

            Debug.Log(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat);
            _chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PlayerProfile.pseudo));
            Application.runInBackground = true;
        }

        private void Start()
        {
            guildScript = guildPageCanvas.GetComponent<GuildScript>();
            guildGroup = guildPageCanvas.GetComponent<CanvasGroup>();
            
            friendChatScript = friendChatCanvas.GetComponent<FriendChatScript>();
            friendChatGroup = friendChatCanvas.GetComponent<CanvasGroup>();

            notificationManagerScript = notificationManagerCanvas.GetComponent<NotificationManagerScript>();
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
            if (PlayerProfile.guildId.HasValue)
            {
                SusbcribeToGuild();
            }
            //_chatClient.SetOnlineStatus(ChatUserStatus.Online);
        }

        public void SusbcribeToGuild()
        {
            if (!PlayerProfile.guildId.HasValue)
                return;

            guildChannelName = $"Guild_{PlayerProfile.guildId}";

            if (_chatClient.PublicChannels.ContainsKey(guildChannelName))
                return;
            if (_chatClient.Subscribe(guildChannelName, messagesFromHistory: 50, creationOptions: new ChannelCreationOptions { MaxSubscribers = 50 }))
                {
                    if (_chatClient.PublicChannels.ContainsKey(guildChannelName))
                    {
                        _chatClient.PublicChannels[guildChannelName].Messages.ForEach(messageObject =>
                        {
                            var messageArray = (string[])messageObject;
                            var sender = messageArray[1];
                            var message = messageArray[1];
                            guildScript.DisplayMessage(message, sender);
                        });
                    }
                    
                    Debug.Log($"Subscribed to {guildChannelName}");
                }
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
                {
                    string sender = message[1];
                    if (guildGroup.alpha == 0 && sender != PlayerProfile.pseudo)
                    {
                        notificationManagerScript.guildMessagesNotification = true;
                        notificationManagerScript.UpdateNotificationObjects();
                    }
                    if (message.Length > 2)
                        guildScript.DisplayLobbyInvitation(message[0], sender, message[2]);
                    else
                        guildScript.DisplayMessage(message[0], sender);
                }
            }
            // All public messages are automatically cached in `Dictionary<string, ChatChannel> PublicChannels`.
            // So you don't have to keep track of them.
            // The channel name is the key for `PublicChannels`.
            // In very long or active conversations, you might want to trim each channels history.
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            Debug.Log($"OnPrivateMessage: {sender} sent {message} to {channelName}");
            var messageObject = (string[])message;
            var messageText = messageObject[0];
            var messageSender = messageObject[1];
            if ((friendChatGroup.alpha == 0 || friendChatScript.currentFriend == null || sender != friendChatScript.currentFriend.name) && sender != PlayerProfile.pseudo)
            {
                notificationManagerScript.friendMessagesSenders.Add(sender);
                notificationManagerScript.UpdateNotificationObjects();
            }
            else if (messageObject.Length > 2)
            {
                friendChatScript.DisplayLobbyInvitation(messageText, messageSender, messageObject[2]);
            }
            else
            {
                friendChatScript.DisplayMessage(messageText, messageSender);
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

        public void SendGuildLobbyInvitation(string roomName)
        {
            string[] message = { "", PlayerProfile.pseudo, roomName };
            _chatClient.PublishMessage(guildChannelName, message);
        }

        public void SendPrivateMessage(string target, string messageText)
        {
            string[] message = { messageText, PlayerProfile.pseudo };
            _chatClient.SendPrivateMessage(target, message);
        }

        public void SendFriendLobbyInvitation(string target, string roomName)
        {
            string[] message = { "", PlayerProfile.pseudo, roomName };
            _chatClient.SendPrivateMessage(target, message);
        }

        public List<string[]> GetPrivateMessages(string target)
        {
            string privateChannelName = _chatClient.GetPrivateChannelNameByUser(target);

            if (_chatClient.TryGetChannel(privateChannelName, true, out ChatChannel privateChannel))
            {
                List<string[]> messages = privateChannel.Messages.Select(messageObject =>
                {
                    return (string[])messageObject;
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