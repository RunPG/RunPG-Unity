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
        public Player player { get; private set; }
        public void SetPlayerInfo(Player newPlayer)
        {
            playerInfo.text = newPlayer.NickName;
        }
    }
}
