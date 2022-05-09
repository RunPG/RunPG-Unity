using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private PlayerDisplay playerDisplayPrefab;
    private List<PlayerDisplay> playersList = new List<PlayerDisplay>();
    [SerializeField]
    private Transform prefabPos;
    [SerializeField]
    private Button leaveButton;
    [SerializeField]
    private Button invitePlayerButton;

    public void Start()
    {
        GetPlayerList();
        leaveButton.onClick.AddListener(LeaveRoom);

    }
    private void GetPlayerList()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            PlayerDisplay newPlayerText = Instantiate(playerDisplayPrefab, prefabPos);
            newPlayerText.SetPlayerInfo(player);
            playersList.Add(newPlayerText);
        }

    }
    #region Photon Callbacks
    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", player.NickName); // not seen if you're the player connecting
        PlayerDisplay newPlayerText = Instantiate(playerDisplayPrefab, prefabPos);
        newPlayerText.SetPlayerInfo(player);
        playersList.Add(newPlayerText);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            LoadLobby();
        }
    }
    public override void OnPlayerLeftRoom(Player player)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", player.NickName); // seen when other disconnects

        int index = playersList.FindIndex(playerDisplay => playerDisplay.player == player);
        if (index != -1)
        {
            Destroy(playersList[index].gameObject);
            playersList.RemoveAt(index);
        }


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            LoadLobby();
        }
    }
    #endregion
    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }


    #region Private Methods


    void LoadLobby()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Lobby");
    }


    #endregion
    #region Public Methods


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    #endregion
}

