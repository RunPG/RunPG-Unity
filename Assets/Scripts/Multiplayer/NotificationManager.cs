using Photon.Pun;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunPG.Multi
{

    public class NotificationManager : MonoBehaviour
    {/*
        [SerializeField]
        private Text allNotificationsText;
        [SerializeField]
        private Text friendRequestCountText;
        [SerializeField]
        private Text lobbyInvitationsText;
        [SerializeField]
        private Text guildInvitationsText;*/
      
        //FriendRequest display
        [SerializeField] Transform friendrequestPrefabPos;
        //GuildDisplay display
        [SerializeField] Transform guildNotificationPrefabPos;

        [SerializeField] Transform lobbyNotificationPrefabPos;

        [SerializeField] Button addFriendButton;
        [SerializeField] InputField friendTextInput;

        [SerializeField] GameObject lobbyInvitationPanel;
        [SerializeField] GameObject DungeonPanel;

        [SerializeField] GameObject notificationPrefab;

        public List<Notification> _notifications = new List<Notification>();

        void Start()
        {
            GetLobbyInvitationAtStart();
            addFriendButton.onClick.AddListener(delegate
            {
                OnclickAddFriend(friendTextInput.text);
            });
            InvokeRepeating(nameof(GetFriendRequests), 0f, 10f);
        }

        void Update()
        {
        }
        //Called at the start of the game and when a friend request is accepted, Instantiate the prefabs
        public void GuildInvitationPrefabInstantiation(string guild, string sender_id)
        {
            var display = Instantiate(notificationPrefab, guildNotificationPrefabPos);
            display.transform.GetChild(0).GetComponent<Text>().text = sender_id;
            display.transform.GetChild(1).GetComponent<Text>().text = guild;
        }
        public void LobbyInvitationPrefabInstantiation(string senderName, int sender_id)
        {
            Debug.Log("SENDER ID" + sender_id);
            var display = Instantiate(notificationPrefab, lobbyNotificationPrefabPos);
            display.transform.GetChild(0).GetComponent<Text>().text = senderName;
            display.AddComponent<LobbyInvitation>();
            display.GetComponent<LobbyInvitation>().sender_id = sender_id;
            display.GetComponent<LobbyInvitation>().lobbyInvitationPanel = lobbyInvitationPanel;
            display.GetComponent<LobbyInvitation>().DungeonPanel = DungeonPanel;

        }
        //Sends a notification to the specified user
        public async void OnclickAddFriend(string friendUsername)
        {
            User friend = await Requests.GETUserByName(friendUsername);
            if (friend != null)
            {
                var coroutine = Requests.POSTSendNotification(GlobalVariables.userId, friend.id, NotificationType.FRIENDLIST);
                StartCoroutine(coroutine);
                Debug.Log("Starting POSTREGISTER" + GlobalVariables.userId);
            }
            else
            {
                Debug.Log("This user does not exist !");
                //TODO: notif error
            }
        }
        async void GetFriendRequests()
        {
            Debug.Log("Getting friend requests");
            var friendRequests = await Requests.GETNotificationsByType(GlobalVariables.userId, NotificationType.FRIENDLIST);
            List<(String, int)> sendersUsername = new List<(String, int)>();
            foreach (var friendRequest in friendRequests)
            {
                if (!_notifications.Where(_friendRequest => _friendRequest.type == friendRequest.type &&  
                _friendRequest.receiverId == friendRequest.receiverId && _friendRequest.receiverId == friendRequest.receiverId && _friendRequest.senderId == friendRequest.senderId).Any())
                {
                    _notifications.Add(friendRequest);
                    var user = await Requests.GETUserById(friendRequest.senderId);
                    sendersUsername.Add((user.name, friendRequest.senderId));
                }
            }
            FriendRequestPrefabInstantiation(sendersUsername);
        }
        //Called at the start of the game, persistence
        async void GetFriendRequestsAtStart()
        {
            var friendRequests = await Requests.GETNotificationsByType(GlobalVariables.userId, NotificationType.FRIENDLIST);
            List<(String, int)> sendersUsername = new List<(String, int)>();
            foreach (var friendRequest in friendRequests)
            {
                var user = await Requests.GETUserById(friendRequest.senderId);
                sendersUsername.Add((user.name, friendRequest.senderId));
            }
            FriendRequestPrefabInstantiation(sendersUsername);
        }
        async void GetGuildInvitationAtStart()
        {
            var guildInvitations = await Requests.GETNotificationsByType(GlobalVariables.userId, NotificationType.GUILD);
            foreach (var guildInvitation in guildInvitations)
            {
                var user = await Requests.GETUserById(guildInvitation.senderId);

                GuildInvitationPrefabInstantiation("", user.name);
            }
        }
        async void GetLobbyInvitationAtStart()
        {
            var lobbyInvitations = await Requests.GETNotificationsByType(GlobalVariables.userId, NotificationType.LOBBY);
            foreach (var lobbyInvitation in lobbyInvitations)
            {
                var user = await Requests.GETUserById(lobbyInvitation.senderId);

                LobbyInvitationPrefabInstantiation(user.name, lobbyInvitation.senderId);
            }
        }
        //Called at the start of the game, Instantiate the prefabs
        void FriendRequestPrefabInstantiation(List<(String, int)> senders)
        {
            foreach (var sender in senders)
            {
                var display = Instantiate(notificationPrefab, friendrequestPrefabPos);
                display.transform.GetChild(0).GetComponent<Text>().text = sender.Item1;
                display.AddComponent<FriendRequest>();
                display.GetComponent<FriendRequest>().sender_id = sender.Item2;

            }
        }
        //Called at the start of the game, persistence
      /*
        void Start()
        {
            var friendRequests = Requests.GETNotificationsByType(Int32.Parse(PhotonNetwork.NickName), NotificationType.FRIENDLIST);
            var lobbyInvitations = Requests.GETNotificationsByType(Int32.Parse(PhotonNetwork.NickName), NotificationType.LOBBY);
            var guildInvitations = Requests.GETNotificationsByType(Int32.Parse(PhotonNetwork.NickName), NotificationType.GUILD);

            var friendRequestsCount = friendRequests.Length;
            var lobbyInvitationsCount = lobbyInvitations.Length;
            var guildInvitationsCount = guildInvitations.Length;

            allNotificationsText.text = "" + friendRequestsCount + lobbyInvitationsCount + guildInvitationsCount;
            friendRequestCountText.text = "" +friendRequestsCount;
            lobbyInvitationsText.text = "" + lobbyInvitationsCount;
            guildInvitationsText.text = "" + guildInvitationsCount;
           
            Debug.Log("Count" + guildInvitations.Length);
        }*/
    } 
}
