using RunPG.Multi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InviteToGuildScript : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField textInput;
    [SerializeField]
    private GameObject userObject;
    [SerializeField]
    private GameObject notFoundText;
    [SerializeField]
    private Button loopButton;
    [SerializeField]
    private GuildScript guildScript;

    private Transform userTransform;


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

        userTransform = userObject.transform;
    }

    async void SearchUser(string username)
    {
        UserModel userModel = await Requests.GETUserByName(username);
        if (userModel == null)
        {
            notFoundText.SetActive(true);
            userObject.SetActive(false);
            return;
        }
        
        if (userModel.guildId == PlayerProfile.guildId)
        {
            notFoundText.SetActive(true);
            userObject.SetActive(false);
        }
        else
        {
            notFoundText.SetActive(false);
            UserCharacterModel userCharacterModel = await Requests.GETUserCharacter(userModel.id);
            userTransform.Find("Class").GetComponent<Image>().sprite = userCharacterModel.character.heroClass.GetSprite();
            userTransform.Find("Name").GetComponent<TextMeshProUGUI>().text = userModel.name;
            userTransform.Find("Level").GetComponent<TextMeshProUGUI>().text = string.Format("Lv. {0}", userCharacterModel.statistics.level);

            NotificationModel notificationModel = await Requests.GETNotificationsByTypeBySender(userModel.id, NotificationType.GUILD, PlayerProfile.id);
            if (notificationModel != null)
            {
                userTransform.Find("AddButton").GetComponent<Image>().sprite = Resources.Load<Sprite>("Social/Check");
            }
            else
            {
                userTransform.Find("AddButton").GetComponent<Image>().sprite = Resources.Load<Sprite>("Social/Add");
                var addButton = userTransform.Find("AddButton").GetComponent<Button>();
                addButton.onClick.RemoveAllListeners();
                addButton.onClick.AddListener(delegate
                {
                    InviteUser(userModel.id);
                });
            }
            userObject.SetActive(true);
        }
    }

    async void InviteUser(int userId)
    {
        await Requests.POSTSendNotification(PlayerProfile.id, userId, NotificationType.GUILD);
        var addButton = userTransform.Find("AddButton");
        addButton.GetComponent<Button>().onClick.RemoveAllListeners();
        addButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Social/Check");
        await guildScript.Load();
    }


}
