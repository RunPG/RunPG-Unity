using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    [Serializable]
    public class FriendModel
    {
        public int userId; 
        public int friendId;

        public FriendModel(int userId, int friendId)
        {
            this.userId = userId;
            this.friendId = friendId;
        }
    }
}
