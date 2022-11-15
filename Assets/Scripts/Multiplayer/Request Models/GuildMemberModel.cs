using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Multiplayer.Request_Models
{
    public class GuildMemberModel
    {
        public int id;
        public string name;
        public bool isOwner;
        public HeroClass heroClass;
        public int level;

        public GuildMemberModel(int id, string name, bool isOwner, HeroClass heroClass, int level)
        {
            this.id = id;
            this.name = name;
            this.isOwner = isOwner;
            this.heroClass = heroClass;
            this.level = level;
        }
    }
}
