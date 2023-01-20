using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace RunPG.Multi
{
  public class PlayerDisplay : MonoBehaviour
  {
    [SerializeField]
    private TextMeshProUGUI playerInfo;
    [SerializeField]
    private Image classImage;
    [SerializeField]
    private TextMeshProUGUI level;
    public Player player { get; private set; }
    public void SetPlayerInfo(Player newPlayer)
    {
      player = newPlayer;
      classImage.sprite = ((HeroClass)newPlayer.CustomProperties["heroClass"]).GetSprite();
      Debug.Log("Level: " + ((int)newPlayer.CustomProperties["level"]).ToString());
      level.text = "Lvl. " + ((int)newPlayer.CustomProperties["level"]).ToString();
      playerInfo.text = newPlayer.NickName;
    }
  }
}
