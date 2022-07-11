using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunPG.Multi
{
    public abstract class Invitation: MonoBehaviour
    {
        public Button acceptButton;
        public Button refuseButton;

        public NotificationManager notificationManager;
        public int sender_id;
        // Start is called before the first frame update
        private void Start()
        {
            acceptButton = transform.Find("Accept Button").GetComponent<Button>();
            refuseButton = transform.Find("Refuse Button").GetComponent<Button>();
            acceptButton.onClick.AddListener(OnClickAccept);
            refuseButton.onClick.AddListener(OnClickRefuse);
        }

        public abstract void OnClickAccept();
        public abstract void OnClickRefuse();
    }
}
