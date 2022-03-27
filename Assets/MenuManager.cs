using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
