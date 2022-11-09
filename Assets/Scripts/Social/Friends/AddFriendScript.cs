using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddFriendScript : MonoBehaviour
{
    [SerializeField]
    private GameObject friendObject;
    [SerializeField]
    private TMP_InputField textInput;
    [SerializeField]
    private GameObject notFoundText;
    [SerializeField]
    private Button loopButton;
    [SerializeField]
    private SocialScript socialScript;

    private Transform friendTransform;

    // Start is called before the first frame update
    void Start()
    {
        textInput.onSubmit.AddListener(delegate
        {
            SearchUser(textInput.text);
        });

        loopButton.onClick.AddListener(delegate
        {
            SearchUser(textInput.text);
        });

        friendTransform = friendObject.transform;

    }

    async void SearchUser(string username)
    {
        UserModel userModel = await Requests.GETUserByName(username);
        if (userModel == null)
        {
            notFoundText.SetActive(true);
            friendObject.SetActive(false);
            return;
        }
        FriendModel friendModel = await Requests.GETUserFriend(PlayerProfile.id, userModel.id);
        if (friendModel != null)
        {
            notFoundText.SetActive(true);
            friendObject.SetActive(false);
        }
        else
        {
            notFoundText.SetActive(false);
            UserCharacterModel userCharacterModel = await Requests.GETUserCharacter(userModel.id);
            friendTransform.Find("Class").GetComponent<Image>().sprite = userCharacterModel.character.heroClass.GetSprite();
            friendTransform.Find("Name").GetComponent<TextMeshProUGUI>().text = userModel.name;
            friendTransform.Find("Level").GetComponent<TextMeshProUGUI>().text = string.Format("Lv. {0}", userCharacterModel.statistics.level);

            NotificationModel notificationModel = await Requests.GETNotificationsByTypeBySender(userModel.id, NotificationType.FRIENDLIST, PlayerProfile.id);
            if (notificationModel != null)
            {
                friendTransform.Find("AddButton").GetComponent<Image>().sprite = Resources.Load<Sprite>("Social/Check");
            }
            else
            {
                friendTransform.Find("AddButton").GetComponent<Image>().sprite = Resources.Load<Sprite>("Social/Add");
                var addButton = friendTransform.Find("AddButton").GetComponent<Button>();
                addButton.onClick.RemoveAllListeners();
                addButton.onClick.AddListener(delegate
                {
                    AddFriend(userModel.id);
                });
            }
            friendObject.SetActive(true);
        }
    }

    async void AddFriend(int friendId)
    {
        await Requests.POSTSendNotification(PlayerProfile.id, friendId, NotificationType.FRIENDLIST);
        var addButton = friendTransform.Find("AddButton");
        addButton.GetComponent<Button>().onClick.RemoveAllListeners();
        addButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Social/Check");
        await socialScript.Load();
    }
}