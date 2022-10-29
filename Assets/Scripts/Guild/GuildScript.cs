using Assets.Scripts.Multiplayer.Request_Models;
using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildScript : MonoBehaviour
{
    [SerializeField]
    private GameObject guildName;
    [SerializeField]
    private GameObject memberListButton;
    [SerializeField]
    private GameObject memberListObject;

    [SerializeField]
    private GameObject textInputObject;
    [SerializeField]
    private GameObject sendObject;
    [SerializeField]
    private GameObject messagesLayout;
    [SerializeField]
    private GameObject messagePrefab;
    [SerializeField]
    private GameObject chatManagerObject;

    public GuildModel guild;
    public GuildMemberModel guildOwner;

    private void Start()
    {
        memberListButton.GetComponent<Button>().onClick.AddListener(async () =>
        {
            var guildMemberListScript = memberListObject.GetComponent<GuildMemberListScript>();
            await LoadGuild();
            guildMemberListScript.ReloadUsersList();
        });

        var chatManagerScript = chatManagerObject.GetComponent<ChatManagerScript>();

        var textInput = textInputObject.GetComponent<TMP_InputField>();
        textInput.onSubmit.AddListener(delegate
        {
            chatManagerScript.SendGuildMessage(textInput.text);
            textInput.text = "";
        });

        var sendButton = sendObject.GetComponent<Button>();
        sendButton.onClick.AddListener(delegate
        {
            chatManagerScript.SendGuildMessage(textInput.text);
            textInput.text = "";
        });
    }

    async Task LoadGuild()
    {
        guild = await Requests.GETGuildById(PlayerProfile.guildId);
        guildOwner = guild.members.Find(user => user.isOwner);
    }

    void AddGuild()
    {
        guildName.GetComponent<TextMeshProUGUI>().text = guild.name;
    }

    public async Task Load()
    {
        await LoadGuild();
        AddGuild();
    }

    public void DisplayMessage(string message, string sender)
    {
        messagePrefab.transform.Find("Sender").GetComponent<TextMeshProUGUI>().text = sender;
        messagePrefab.transform.Find("MessageZone/Message").GetComponent<TextMeshProUGUI>().text = message;
        Instantiate(messagePrefab, messagesLayout.transform);
    }
}
