using Photon.Pun;
using Photon.Realtime;
using RunPG.Multi;
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
    private Button joinButton;
    [SerializeField]
    private Button leaveButton;
    [SerializeField]
    private Button StartButton;
    [SerializeField]
    private GameObject DungeonPanel;

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

    string gameVersion = "1";
    public void Start()
    {
        leaveButton.onClick.AddListener(LeaveRoom);
        joinButton.onClick.AddListener(CreateRoom);
        StartButton.onClick.AddListener(LoadDungeon);
    }
    public void LoadDungeon()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("PhotonNetwork : Loading Dungeon");
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel("DungeonScene");
        }
    } /*
    [PunRPC]
    public void LoadDungeonRPC()
    {
        Debug.LogFormat("PhotonNetwork : Loading Dungeon");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("DungeonScene");
   
    }
    */
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom("" + GlobalVariables.userId, roomOptions, TypedLobby.Default);
    }

    public void ConnectToRoom(string roomName)
    {
        if (roomName == null)
            Debug.Log("Launcher: ConnectToRoom() roomName is null");
        if (!PhotonNetwork.JoinRoom(roomName))
            Debug.Log("Launcher: ConnectToRoom() failed");
        else
            Debug.Log("Launcher: ConnectToRoom() worked");
    }
    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    /// 

    public override void OnCreatedRoom()
    {
        Debug.Log("room created");
        base.OnCreatedRoom();
    }
    public void Connect(RoomInfo roomInfo)
    {
        Debug.Log("Connect called");
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRoom(roomInfo.Name);
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
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
        }
        else
        {
            if (player.IsMasterClient)
            {
                LeaveRoom();
            }
        }
    }
    #endregion
    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        Debug.Log("left room");
    }


    #region Private Methods




    #endregion
    #region Public Methods


    public void LeaveRoom()
    {/*
        GameObject.Find("DungeonPanel").SetActive(false);
        GameObject.Find("Main Panel").SetActive(true);
        */
        if (DungeonPanel.activeSelf)
            PhotonNetwork.LeaveRoom();
    }
    public override void OnJoinedRoom()
    {
        GetPlayerList();
        Debug.Log("Now this client is in a room.");
    }
    
    #endregion
}

