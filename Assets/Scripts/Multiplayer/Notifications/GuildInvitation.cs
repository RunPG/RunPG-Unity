using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunPG.Multi
{
    public class GuildInvitation : Invitation
    {
        public override void OnClickAccept()
        {/*
            StartCoroutine(Requests.DELETENotification(Int32.Parse(PhotonNetwork.NickName), sender_id, NotificationType.GUILD));
            StartCoroutine(Requests.POSTAddFriend(Int32.Parse(PhotonNetwork.NickName), sender_id));
            StartCoroutine(Requests.POSTAddFriend(sender_id, Int32.Parse(PhotonNetwork.NickName)));
            Destroy(gameObject);
            chatManager.GuildInvitationPrefabInstantiation(transform.GetChild(0).GetComponent<Text>().text, transform.GetChild(1).GetComponent<Text>().text);
        */
        }

        public override void OnClickRefuse()
        {
            StartCoroutine(Requests.DELETENotification(Int32.Parse(PhotonNetwork.NickName), sender_id, NotificationType.GUILD));
            Destroy(gameObject);
        }
    }
}