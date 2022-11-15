using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NotificationManagerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject menuNotificationsObject;
    [SerializeField]
    private GameObject friendNotificationsObject;
    
    [SerializeField]
    private GameObject guildNotificationsObject;
    [SerializeField]
    private GameObject guildListNotificationsObject;

    [SerializeField]
    private SocialScript socialScript;

    NotificationModel[] friendNotifications;
    NotificationModel[] guildNotifications;

    public bool guildMessagesNotification = false;

    public HashSet<string> friendMessagesSenders;
    
    void Start()
    {
        friendNotifications = new NotificationModel[0];
        guildNotifications = new NotificationModel[0];
        friendMessagesSenders = new HashSet<string>();
        InvokeRepeating(nameof(UpdateNotifications), 0, 30);
    }

    public async Task UpdateNotifications()
    {
        Debug.Log("UpdateNotifications");
        await GetAllNotifications();
        UpdateNotificationObjects();
    }

   async Task GetAllNotifications()
    {
        friendNotifications = await Requests.GETNotificationsByType(PlayerProfile.id, NotificationType.FRIENDLIST);
        guildNotifications = await Requests.GETNotificationsByType(PlayerProfile.id, NotificationType.GUILD);
    }

    public void UpdateNotificationObjects()
    {
        bool showFriendNotifications = friendNotifications.Length > 0 || friendMessagesSenders.Count > 0;
        bool showGuildNotifications = guildNotifications.Length > 0 || guildMessagesNotification;

        menuNotificationsObject.SetActive(showFriendNotifications || showGuildNotifications);
        friendNotificationsObject.SetActive(showFriendNotifications);
        guildNotificationsObject.SetActive(showGuildNotifications);
        guildListNotificationsObject.SetActive(guildNotifications.Length > 0);
        //lobbyNotificationsObject.SetActive(showLobbyNotifications);

        foreach (var sender in friendMessagesSenders)
        {
            socialScript.AddMessageNotification(sender);
        }
    }
}
