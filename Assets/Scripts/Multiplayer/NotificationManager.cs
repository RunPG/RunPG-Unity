using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunPG.Multi
{

    public class NotificationManager : MonoBehaviour
    {
        [SerializeField]
        private Text allNotificationsText;
        [SerializeField]
        private Text friendRequestCountText;
        [SerializeField]
        private Text lobbyInvitationsText;
        [SerializeField]
        private Text guildInvitationsText;
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
        }
    } 
}
