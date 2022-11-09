using Assets.Scripts.Multiplayer.Request_Models;
using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildScript : IChat
{
    [SerializeField]
    private TextMeshProUGUI guildName;
    [SerializeField]
    private Button memberListButton;
    [SerializeField]
    private GuildMemberListScript guildMemberListScript;

    [SerializeField]
    private TMP_InputField textInput;
    [SerializeField]
    private Button sendButton;
    [SerializeField]
    private ChatManagerScript chatManagerScript;

    public GuildModel guild;
    public GuildMemberModel guildOwner;

    private void Start()
    {
        memberListButton.onClick.AddListener(async () =>
        {
            await LoadGuild();
            guildMemberListScript.ReloadUsersList();
        });

        textInput.onSubmit.AddListener(delegate
        {
            chatManagerScript.SendGuildMessage(textInput.text);
            textInput.text = "";
        });

        sendButton.onClick.AddListener(delegate
        {
            chatManagerScript.SendGuildMessage(textInput.text);
            textInput.text = "";
        });
    }

    async Task LoadGuild()
    {
        if (PlayerProfile.guildId.HasValue)
        {
            guild = await Requests.GETGuildById(PlayerProfile.guildId.Value);
            guildOwner = guild.members.Find(user => user.isOwner);
        }      
    }

    void AddGuild()
    {
        guildName.text = guild.name;
    }

    public async Task Load()
    {
        await LoadGuild();
        AddGuild();
    }
}
