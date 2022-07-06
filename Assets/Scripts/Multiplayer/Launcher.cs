using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RunPG.Multi
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        #endregion


        #region Private Fields
        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting;

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";


        #endregion


        #region MonoBehaviour CallBacks

        private void Start()
        {

        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        #endregion
        #region Public Methods
        /// <summary>
        /// Connect to a room according to the name passed in argument
        /// </summary>
        /// <param name="roomName"></param>
        /// <exception cref="System.Exception"></exception>
        public void ConnectToRoom(string roomName)
        {
            if (roomName == null)
                Debug.Log("Launcher: ConnectToRoom() roomName is null");
            if (!PhotonNetwork.JoinRoom(roomName))
                Debug.Log("Launcher: ConnectToRoom() failed");
            else
                Debug.Log("Launcher: ConnectToRoom() worked");
        }
        public override void OnCreatedRoom()
        {
            Debug.Log("room created");
            base.OnCreatedRoom();
        }
        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
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
        #endregion
        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            isConnecting = false;
        }

        /* private void Update()
         {
             //Now that we are connected, we can load the main scene.
             if (_loadingOperation != null)
                 _progressSilder.value = _loadingOperation.progress;
         }*/
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
            isConnecting = false;
        }
    }
    #endregion
}