using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

namespace RunPG.Multi
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
 

        [Tooltip("The UI Label to inform the user that the loading is in progress")]
        [SerializeField]
        byte maxPlayersPerRoom = 4;
        private AsyncOperation _loadingOperation;
        [SerializeField]
        private Slider _progressSilder;
        [SerializeField]
        private InputField _username;
        [SerializeField]
        private Button _connexionButton;
        [SerializeField]
        private Button _registerButton;
        [SerializeField]
        private GameObject _connexionPannel;
        [SerializeField]
        private GameObject _authentificationPannel;
        [SerializeField]
        private GameObject _errorMessage ;
        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>

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
            _connexionButton.onClick.AddListener(GoToMainMenu);
            _registerButton.onClick.AddListener(() => StartCoroutine("Register"));

        }
        bool GETPlayer()
        {
            if (_username.text.Length != 0)
            {
                using (UnityWebRequest request = UnityWebRequest.Get("http://178.62.237.73/user/" + _username.text))
                {
                    request.SendWebRequest();
                    while (request.isDone)
                    {
                        //TODO change 
                        //waiting for request to be done
                    }
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        _errorMessage.SetActive(true);
                        _errorMessage.GetComponent<Text>().text = "User does not exist !";

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }             
            }
            return false;
        }
        private void GoToMainMenu()
        {
            if (isConnecting || !GETPlayer())
                return;
            _authentificationPannel.SetActive(false);
            _connexionPannel.SetActive(true);
            PhotonNetwork.NickName = _username.text;
            PhotonNetwork.JoinLobby();
            _loadingOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
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

        private void Update()
        {
            //Now that we are connected, we can load the main scene.
            if (_loadingOperation != null)
                _progressSilder.value = _loadingOperation.progress;


        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
            isConnecting = false;
        }
        public void CreateRoom()
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = maxPlayersPerRoom;
            // null means we dont want a special room name
            PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
        }
        IEnumerator Register()
        {
            if (_username.text.Length != 0)
            {
                var str = "{\"name\":\"" + _username.text + "\"}";
                Debug.Log(str);
                using (UnityWebRequest request = UnityWebRequest.Post("http://178.62.237.73/user/", "POST"))
                {
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(str)) as UploadHandler;
                    yield return request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        _errorMessage.SetActive(true);
                        _errorMessage.GetComponent<Text>().text = "User already exist!";
                    }             
                }
            }
        }
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            print("Launcher: CreateRoom failed: " + message + " return code:" + returnCode); 
        }
      /*  async Task<string> makeRequest()
        {

            var json = "{\"name\":\"test" + "\"}";
            using (UnityWebRequest webRequest = UnityWebRequest.Post("http://178.62.237.73/user/", json))
            {
                webRequest.SendWebRequest();

                string result = Encoding.UTF8.GetString(webRequest.downloadHandler.data);

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    JSONNode error = JSON.Parse(result);

                    registerAndLoginError.GetComponent<Text>().text = error["error"]["message"];
                    registerAndLoginError.SetActive(true);

                    return "";
                }
            }

            BasicLogin();
            return "";
        }*/
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
    
            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

       public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the Lobby");


                // #Critical
                // Load the lobby.
                PhotonNetwork.LoadLevel("Lobby");
            }
        }
        #endregion
    }
}