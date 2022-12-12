using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IChat : MonoBehaviour
{
    [SerializeField]
    protected GameObject messagePrefab;
    [SerializeField]
    protected GameObject lobbyInvitationPrefab;

    [SerializeField]
    protected GameObject messagesLayout;
    [SerializeField]
    protected LobbyManager lobbyManager;

    public void DisplayMessage(string message, string sender)
    {
        var messageObject = Instantiate(messagePrefab, messagesLayout.transform).transform;
        messageObject.Find("Sender").GetComponent<TextMeshProUGUI>().text = sender;
        messageObject.Find("MessageZone/Message").GetComponent<TextMeshProUGUI>().text = message;
    }

    public void DisplayLobbyInvitation(string message, string sender, string roomName)
    {
        var messageObject = Instantiate(lobbyInvitationPrefab, messagesLayout.transform).transform;
        messageObject.Find("Sender").GetComponent<TextMeshProUGUI>().text = sender;
        messageObject.Find("MessageZone/Message").GetComponent<TextMeshProUGUI>().text = $"{sender} invite you to {roomName}";
        var joinButton = messageObject.Find("JoinButton").GetComponent<Button>();
        joinButton.onClick.AddListener(delegate
        {
            if (!lobbyManager.ConnectToRoom(roomName))
            {
                joinButton.interactable = false;
                return;
            }

            var CanvasGroup = GetComponent<CanvasGroup>();
            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        });
    }

    public void ClearChat()
    {
        foreach (Transform child in messagesLayout.transform)
        {
            Destroy(child.gameObject);
        }
    }
}