using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunPG.Multi
{
    public abstract class NotificationDisplay : MonoBehaviour
    {
        [SerializeField]
        private Button acceptButton;
        [SerializeField]
        private Button refuseButton;

        public ChatManager chatManager;
        public int sender_id;

        // Start is called before the first frame update
        private void Start()
        {
            acceptButton.onClick.AddListener(OnClickAccept);
            refuseButton.onClick.AddListener(OnClickRefuse);
        }

        public abstract void OnClickAccept();
        public abstract void OnClickRefuse();
    }
}
