using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using Photon.Pun;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient _chatClient;
    [SerializeField] InputField textInput;
    [SerializeField] Button sendMsgButton;
    [SerializeField] GameObject msgPrefab;
    [SerializeField] GameObject prefabPos;

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange:OnConnected called");

    }

    public void OnConnected()   
    {
        Debug.Log("ChatManager:OnConnected called");
        _chatClient.Subscribe("guild", creationOptions: new ChannelCreationOptions { MaxSubscribers = 50 });
        _chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnDisconnected()
    {
        Debug.Log("ChatManager:OnDisconnected called");

    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        Debug.Log("OnGetMessages");

        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {

            msgs = string.Format("{0}{1}={2}, ", msgs, senders[i], messages[i]);
            if (senders[i] != PhotonNetwork.NickName)
                DisplayMessage(messages[i].ToString(), senders[i]);


        }
        // All public messages are automatically cached in `Dictionary<string, ChatChannel> PublicChannels`.
        // So you don't have to keep track of them.
        // The channel name is the key for `PublicChannels`.
        // In very long or active conversations, you might want to trim each channels history.
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("ChatManager:OnStatusUpdate called: " + user + status);


    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log("ChatManager: OnUserSubscribed called");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
    private void Awake()
    {
        _chatClient = new ChatClient(this);
        _chatClient.ChatRegion = "EU";

        Debug.Log(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat);
        _chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(PhotonNetwork.NickName));
        Application.runInBackground = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        sendMsgButton.onClick.AddListener(SendMessage);

    }
    void SendMessage()
    {
        DisplayMessage(textInput.text, PhotonNetwork.NickName);
        _chatClient.PublishMessage("guild", textInput.text);
        textInput.text = "";

    }
    public void DisplayMessage(string message, string sender)
    {
        var msg = Instantiate(msgPrefab, prefabPos.transform);
        msg.transform.GetChild(0).GetComponent<Text>().text = message;
        msg.transform.GetChild(1).GetComponent<Text>().text = sender;
    }
    // Update is called once per frame
    void Update()
    {
        _chatClient.Service();
    }
}
