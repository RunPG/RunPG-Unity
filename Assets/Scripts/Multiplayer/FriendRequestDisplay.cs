using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunPG.Multi
{
    public class FriendRequestDisplay : NotificationDisplay
    {
        public override void OnClickAccept()
        {
            StartCoroutine(Requests.DELETENotification(Int32.Parse(PhotonNetwork.NickName), sender_id, NotificationType.FRIENDLIST));
            StartCoroutine(Requests.POSTAddFriend(Int32.Parse(PhotonNetwork.NickName), sender_id));
            StartCoroutine(Requests.POSTAddFriend(sender_id, Int32.Parse(PhotonNetwork.NickName)));
            Destroy(gameObject);
            String[] arr = { transform.GetChild(0).GetComponent<Text>().text};
            chatManager.FriendlistPrefabInstantiation(arr);
        }
        public override void OnClickRefuse()
        {
            StartCoroutine(Requests.DELETENotification(Int32.Parse(PhotonNetwork.NickName), sender_id, NotificationType.FRIENDLIST));
            Destroy(gameObject);
        }
    }
}