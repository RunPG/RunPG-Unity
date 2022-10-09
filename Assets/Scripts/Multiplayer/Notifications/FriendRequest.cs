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

        public override async void OnClickAccept()
        {
            await Requests.DELETENotification(GlobalVariables.userId, sender_id, NotificationType.FRIENDLIST);
            await Requests.POSTAddFriend(GlobalVariables.userId, sender_id);
            await Requests.POSTAddFriend(sender_id, GlobalVariables.userId);
            Destroy(gameObject);
            List<(String,int)> friend = new List<(String,int)> ();
            friend.Add((transform.GetChild(0).GetComponent<Text>().text, sender_id));
            GameObject.FindObjectOfType<ChatManager>().friendDisplayPrefabInstantiation(friend);
        }

        public override async void OnClickRefuse()
        {
            await Requests.DELETENotification(GlobalVariables.userId, sender_id, NotificationType.FRIENDLIST);
            Destroy(gameObject);
        }
    }
}