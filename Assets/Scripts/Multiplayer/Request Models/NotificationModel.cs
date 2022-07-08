using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    public enum NotificationType
    {
        FRIENDLIST,
        LOBBY,
        GUILD
    }
    public class NotificationModel
    {
        public int senderId;
        public int receiverId;
        public NotificationType type;
    }
}
