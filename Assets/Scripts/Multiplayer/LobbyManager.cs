using Photon.Pun;
using Photon.Realtime;
using RunPG.Multi;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] RoomDisplay roomDisplayPrefab;
    [SerializeField] private Button createButton;
    [SerializeField] private Button leaveButtonRoom;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button startButton;

    [SerializeField] private CanvasGroup lobbyList;
    [SerializeField] private CanvasGroup lobby;
    [SerializeField] private CanvasGroup portalDescription;
    [SerializeField] private CanvasGroup InventoryCanvas;
    [SerializeField] private CanvasGroup canvas;

    [SerializeField]
    private Transform lobbyPrefabPos;
    [SerializeField]
    private Transform playerPrefabPos;
    [SerializeField]
    private PlayerDisplay playerDisplayPrefab;

    [SerializeField]
    private PlayerMovement playerMovement;

    private List<PlayerDisplay> playersList = new List<PlayerDisplay>();
    private List<RoomDisplay> roomDisplayListing = new List<RoomDisplay>();

    private long poiId;
    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

    string gameVersion = "1";

    public static LobbyManager instance;
    public void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        isConnecting = PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void FindDungeonLobbies(long poiId)
    {
        lobbyList.alpha = 1;
        lobbyList.interactable = true;
        lobbyList.blocksRaycasts = true;
        lobby.alpha = 0;
        lobby.interactable = false;
        lobby.blocksRaycasts = false;
        portalDescription.alpha = 0;
        portalDescription.interactable = false;
        portalDescription.blocksRaycasts = false;
        canvas.alpha = 0;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
        playerMovement.SetUIState(true);
        this.poiId = poiId;
    }

    public void Start()
    {
        leaveButtonRoom.onClick.AddListener(LeaveRoom);
        closeButton.onClick.AddListener(Close);
        createButton.onClick.AddListener(CreateRoom);
        startButton.onClick.AddListener(LoadDungeon);
    }
    public void LoadDungeon()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("PhotonNetwork : Loading Dungeon");
            photonView.RPC("UseActivity", RpcTarget.All);
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel("DungeonScene");
        }
    }

    [PunRPC]
    async void UseActivity()
    {
        await Requests.POSTActivity(PlayerProfile.id, poiId);
    }

    public void Close()
    {
        lobbyList.alpha = 0;
        lobbyList.interactable = false;
        lobbyList.blocksRaycasts = false;

        canvas.alpha = 1;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;

        playerMovement.SetUIState(false);
    }
    /*
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
        roomOptions.PlayerTtl = 0;
        //todo change room name
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName, roomOptions, TypedLobby.Default);
    }

    public void Displaylobby()
    {
        lobbyList.alpha = 0;
        lobbyList.interactable = false;
        lobbyList.blocksRaycasts = false;

        canvas.alpha = 0;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        lobby.alpha = 1;
        lobby.interactable = true;
        lobby.blocksRaycasts = true;
    }

    public void ConnectToRoom(string roomName)
    {
        if (roomName == null)
            Debug.Log("Launcher: ConnectToRoom() roomName is null");
        if (!PhotonNetwork.JoinRoom(roomName))
            Debug.Log("Launcher: ConnectToRoom() failed");
        else
        {
            Debug.Log("Launcher: ConnectToRoom() worked");
            startButton.interactable = false;
        }
    }
    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    /// 
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");

        if (PlayerProfile.pseudo != null)
        {
            PhotonNetwork.NickName = PlayerProfile.pseudo;
            Hashtable hash = new Hashtable();
            hash.Add("heroClass", PlayerProfile.characterInfo.heroClass);
            hash.Add("level", PlayerProfile.characterInfo.level);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        else
            PhotonNetwork.NickName = "test dev";
        PhotonNetwork.JoinLobby();
        isConnecting = false;
    }

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
            PlayerDisplay newPlayerText = Instantiate(playerDisplayPrefab, playerPrefabPos);
            newPlayerText.SetPlayerInfo(player);
            playersList.Add(newPlayerText);
        }

    }
    #region Photon Callbacks
    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", player.NickName); // not seen if you're the player connecting
        int index = playersList.FindIndex(playerDisplay => playerDisplay.player == player);
        if (index == -1)
        {
            PlayerDisplay newPlayerText = Instantiate(playerDisplayPrefab, playerPrefabPos);
            newPlayerText.SetPlayerInfo(player);
            playersList.Add(newPlayerText);
        }
    }
    // not seen if you're the player connecting
    /*layerDisplay newPlayerText = Instantiate(playerDisplayPrefab, prefabPos);
     newPlayerText.SetPlayerInfo(player);
     playersList.Add(newPlayerText);/*
 }*/
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
        roomDisplayListing.ForEach(roomDisplay => roomDisplay.gameObject.Destroy());
        roomDisplayListing.Clear();
        playersList.ForEach(roomDisplay => roomDisplay.gameObject.Destroy());
        playersList.Clear();
        startButton.interactable = true;
    }
    #region Private Methods

    #endregion
    #region Public Methods

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                int index = roomDisplayListing.FindIndex(x => x.roomInfo.Name == room.Name);
                if (index != -1)
                {
                    Destroy(roomDisplayListing[index].gameObject);
                    roomDisplayListing.RemoveAt(index);
                }
            }
            else
            {
                if (!roomDisplayListing.Find(roomDisplay => roomDisplay.roomInfo.Name == room.Name))
                {
                    RoomDisplay newRoomDisplay = Instantiate(roomDisplayPrefab, lobbyPrefabPos);
                    if (newRoomDisplay)
                    {
                        newRoomDisplay.SetRoomInfo(room);
                        newRoomDisplay.joinLobbyButton.onClick.AddListener(delegate
                        {
                            ConnectToRoom(room.Name);
                        });
                        roomDisplayListing.Add(newRoomDisplay);
                    }
                }
            }
        }
    }
    public void LeaveRoom()
    {
        Debug.Log("Leave room");
        PhotonNetwork.LeaveRoom();

        FindDungeonLobbies();
    }
    public override void OnJoinedRoom()
    {
        //GetPlayerList();
        Displaylobby();
        Debug.Log("Now this client is in a room.");
        GetPlayerList();
    }

    #endregion
}

