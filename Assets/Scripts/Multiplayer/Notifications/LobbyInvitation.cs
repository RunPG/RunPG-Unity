using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunPG.Multi
{
    public class LobbyInvitation : Invitation
    {

        [SerializeField] public GameObject lobbyInvitationPanel;
        [SerializeField] public GameObject DungeonPanel;

        private LobbyManager _lobbyManager;

        private void Start()
        {
            _lobbyManager = GameObject.FindObjectOfType<LobbyManager>();
            acceptButton = transform.Find("Accept Button").GetComponent<Button>();
            refuseButton = transform.Find("Refuse Button").GetComponent<Button>();
            acceptButton.onClick.AddListener(OnClickAccept);
            refuseButton.onClick.AddListener(OnClickRefuse);
        }
        public override void OnClickAccept()
        {
            Debug.Log("");
            StartCoroutine(Requests.DELETENotification(GlobalVariables.userId, sender_id, NotificationType.LOBBY));
            lobbyInvitationPanel.SetActive(false);
            DungeonPanel.SetActive(true);
            //TODO: find another room names
            _lobbyManager.ConnectToRoom(""+ sender_id);
            Destroy(gameObject);
        }

        public override void OnClickRefuse()
        {
            StartCoroutine(Requests.DELETENotification(GlobalVariables.userId, sender_id, NotificationType.GUILD));
            Destroy(gameObject);
        }
    }
}