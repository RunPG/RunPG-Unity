using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunPG.Multi
{
    public class FriendRequestDisplay : MonoBehaviour
    {
        [SerializeField]
        private Button acceptButton;
        [SerializeField]
        private Button refuseButton;

        public ChatManager chatManager;

        public int friend_id;
        private void Start()
        {
            acceptButton.onClick.AddListener(OnClickAccept);
            refuseButton.onClick.AddListener(OnClickRefuse);
        }
        void OnClickAccept()
        {
            Requests.DELETENotification(Int32.Parse(PhotonNetwork.NickName), friend_id);
            StartCoroutine(Requests.POSTAddFriend(Int32.Parse(PhotonNetwork.NickName), friend_id));
            StartCoroutine(Requests.POSTAddFriend(friend_id,Int32.Parse(PhotonNetwork.NickName)));
            Destroy(gameObject);
            String[] arr = { transform.GetChild(0).GetComponent<Text>().text};
            chatManager.FriendlistPrefabInstantiation(arr);
        }
        void OnClickRefuse()
        {
            Requests.DELETENotification(Int32.Parse(PhotonNetwork.NickName), friend_id);
            Destroy(gameObject);
        }
    }
}