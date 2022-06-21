using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Button mainButton;
    [SerializeField]
    private Button CancelButton;
    [SerializeField]
    private Button FriendListButton;
    [SerializeField]
    private Button CharacterButton;
    [SerializeField]
    private Button guildButton;
    [SerializeField]
    private Button lobbyButton;
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private GameObject GuildPanel;
    [SerializeField]
    private GameObject CharacterPanel;
    [SerializeField]
    private GameObject FriendListPanel;

    // Start is called before the first frame update
    void Start()
    {
        mainButton.onClick.AddListener(delegate {
        menuPanel.SetActive(true);
        mainPanel.SetActive(false);
        });
        CancelButton.onClick.AddListener(delegate {
            menuPanel.SetActive(false);
            GuildPanel.SetActive(false);
            CharacterPanel.SetActive(false);
            FriendListPanel.SetActive(false);
            mainPanel.SetActive(true);
        });
        guildButton.onClick.AddListener(delegate {
            menuPanel.SetActive(false);
            GuildPanel.SetActive(true);
        });
        CharacterButton.onClick.AddListener(delegate {
            menuPanel.SetActive(false);
            CharacterPanel.SetActive(true);
        });
        FriendListButton.onClick.AddListener(delegate {
            menuPanel.SetActive(false);
            FriendListPanel.SetActive(true);
        });
        lobbyButton.onClick.AddListener(JoinLobby);
    }
    void JoinLobby()
    {
       
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 4;
        // null means we dont want a special room name
        PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
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
}
