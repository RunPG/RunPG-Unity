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
}
