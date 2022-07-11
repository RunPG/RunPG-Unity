using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    public class UserModel
    {
        public int id;
        public string uid;
        public string name;
        public int? guildId;
        public int? characterId;
        public string lastCaloriesUpdate;
        public bool isGuildOwner;

        public UserModel(int id, string uid, string name, int guildId, int characterId, string lastCaloriesUpdate, bool isGuildOwner)
        {
            this.id = id;
            this.uid = uid;
            this.name = name;
            this.guildId = guildId;
            this.characterId = characterId;
            this.lastCaloriesUpdate = lastCaloriesUpdate;
            this.isGuildOwner = isGuildOwner;
        }
    }

}
