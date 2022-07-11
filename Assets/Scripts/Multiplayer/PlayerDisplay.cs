using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Player player { get; private set; }
        public void SetPlayerInfo(Player newPlayer)
        {
            player = newPlayer;
            classImage.sprite = ((HeroClass)newPlayer.CustomProperties["heroClass"]).GetSprite();
            playerInfo.text = newPlayer.NickName;
        }
    }
}
