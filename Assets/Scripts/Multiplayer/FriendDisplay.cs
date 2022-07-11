using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunPG.Multi
{
    public class FriendDisplay : MonoBehaviour
    {
        [SerializeField] Button lobbyInvitationButton;
       // [SerializeField] Button chatButton;
        public int _friendId;

        // Start is called before the first frame update
        void Start()
        {
            lobbyInvitationButton.onClick.AddListener(OnClickSendLobbyInvitation);
        }

        void OnClickSendLobbyInvitation()
        {
            Debug.Log("Sending lobby invitation friendID:" + _friendId + " userid:" + GlobalVariables.userId);  
            StartCoroutine(Requests.POSTSendNotification(GlobalVariables.userId, _friendId, NotificationType.LOBBY));
        }
    }
}