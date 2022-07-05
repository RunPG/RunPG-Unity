using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunPG.Multi
{
    public class FriendRequest : Invitation
    {
        public override void OnClickAccept()
        {
            StartCoroutine(Requests.DELETENotification(GlobalVariables.userId, sender_id, NotificationType.FRIENDLIST));
            StartCoroutine(Requests.POSTAddFriend(GlobalVariables.userId, sender_id));
            StartCoroutine(Requests.POSTAddFriend(sender_id, GlobalVariables.userId));
            Destroy(gameObject);
            List<(String,int)> friend = new List<(String,int)> ();
            friend.Add((transform.GetChild(0).GetComponent<Text>().text, sender_id));
            chatManager.friendDisplayPrefabInstantiation(friend);
        }

        public override void OnClickRefuse()
        {
            StartCoroutine(Requests.DELETENotification(GlobalVariables.userId, sender_id, NotificationType.FRIENDLIST));
            Destroy(gameObject);
        }
    }
}