using Photon.Chat;
using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendChatScript : MonoBehaviour
{
    [SerializeField]
    private GameObject friendNameObject;
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

    private ChatManagerScript chatManagerScript;
    
    public UserInfos currentFriend;
    

    // Start is called before the first frame update
    void Start()
    {
        chatManagerScript = chatManagerObject.GetComponent<ChatManagerScript>();

        var textInput = textInputObject.GetComponent<TMP_InputField>();
        textInput.onSubmit.AddListener(delegate
        {
            chatManagerScript.SendPrivateMessage(currentFriend.name, textInput.text);
            AddMessage(textInput.text, PlayerProfile.pseudo);
            textInput.text = "";
        });

        var sendButton = sendObject.GetComponent<Button>();
        sendButton.onClick.AddListener(delegate
        {
            chatManagerScript.SendPrivateMessage(currentFriend.name, textInput.text);
            AddMessage(textInput.text, PlayerProfile.pseudo);
            textInput.text = "";
        });
    }

    void ClearMessagelist()
    {
        foreach (Transform child in messagesLayout.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void LoadMessages(List<string[]> messages)
    {
        ClearMessagelist();
        foreach (var message in messages)
        {
            AddMessage(message[0], message[1]);
        }
    }

    public void AddMessage(string message, string sender)
    {
        var messageObject = Instantiate(messagePrefab, messagesLayout.transform).transform;
        messageObject.Find("Sender").GetComponent<TextMeshProUGUI>().text = sender;
        messageObject.Find("MessageZone/Message").GetComponent<TextMeshProUGUI>().text = message;
    }

    public void Connect(UserInfos friend)
    {
        currentFriend = friend;
        friendNameObject.GetComponent<TextMeshProUGUI>().text = friend.name;
        List<string[]> messages = chatManagerScript.GetPrivateMessages(friend.name);
        LoadMessages(messages);
    }
}
