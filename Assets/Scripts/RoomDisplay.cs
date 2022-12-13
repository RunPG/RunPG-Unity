using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomDisplay : MonoBehaviour
{
  [SerializeField]
  public Button joinLobbyButton;
  [SerializeField]
  private TextMeshProUGUI numberOfPlayer;
  [SerializeField]
  private TextMeshProUGUI hostNameText;
  public RoomInfo roomInfo { get; private set; }

  public void SetRoomInfo(RoomInfo roomInfo)
  {
    this.roomInfo = roomInfo;
    numberOfPlayer.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
    hostNameText.text = roomInfo.Name;
  }
}
