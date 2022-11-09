using RunPG.Multi;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendChatScript : IChat
{
    [SerializeField]
    private TextMeshProUGUI friendNameText;
    [SerializeField]
    private TMP_InputField textInput;
    [SerializeField]
    private Button sendButton;
    [SerializeField]
    private ChatManagerScript chatManagerScript;
    
    public UserInfos currentFriend;
    

    // Start is called before the first frame update
    void Start()
    {
        textInput.onSubmit.AddListener(delegate
        {
            chatManagerScript.SendPrivateMessage(currentFriend.name, textInput.text);
            DisplayMessage(textInput.text, PlayerProfile.pseudo);
            textInput.text = "";
        });

        sendButton.onClick.AddListener(delegate
        {
            chatManagerScript.SendPrivateMessage(currentFriend.name, textInput.text);
            DisplayMessage(textInput.text, PlayerProfile.pseudo);
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
            if (message.Length == 2)
            {
                DisplayMessage(message[0], message[1]);
            }
            else
            {
                DisplayLobbyInvitation(message[0], message[1], message[2]);
            }
        }
    }

    public void Connect(UserInfos friend)
    {
        currentFriend = friend;
        friendNameText.text = friend.name;
        List<string[]> messages = chatManagerScript.GetPrivateMessages(friend.name);
        LoadMessages(messages);
    }
}
