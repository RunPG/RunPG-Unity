using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private Button dungeonButton;
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
    [SerializeField]
    private GameObject DungeonPanel;
    [SerializeField]
    private GameObject LobbyInvitationPanel;
    [SerializeField]
    private Button LobbyInvitationButton;
    // Start is called before the first frame update
    void Start()
    {
        mainButton.onClick.AddListener(delegate
        {
            menuPanel.SetActive(true);
            mainPanel.SetActive(false);
        });
        CancelButton.onClick.AddListener(delegate
        {
            menuPanel.SetActive(true);
            GuildPanel.SetActive(false);
            CharacterPanel.SetActive(false);
            FriendListPanel.SetActive(false);
            DungeonPanel.SetActive(false);
            LobbyInvitationPanel.SetActive(false);
        });
        guildButton.onClick.AddListener(delegate
        {
            menuPanel.SetActive(false);
            GuildPanel.SetActive(true);
        });
        CharacterButton.onClick.AddListener(delegate
        {
            menuPanel.SetActive(false);
            CharacterPanel.SetActive(true);
        });
        FriendListButton.onClick.AddListener(delegate
        {
            menuPanel.SetActive(false);
            FriendListPanel.SetActive(true);
        });
        dungeonButton.onClick.AddListener((delegate
        {
            SceneManager.LoadScene("LobbyBis");
        }));
        LobbyInvitationButton.onClick.AddListener((delegate
        {
            menuPanel.SetActive(false);
            LobbyInvitationPanel.SetActive(true);
        }));

    }
    
}
