using Assets.Scripts.Multiplayer.Request_Models;
using System;
using System.Collections.Generic;

namespace RunPG.Multi
{
    [Serializable]
    public class GuildModel
    {
        public int id;
        public string name;
        public string description;
        public List<GuildMemberModel> members;

        public GuildModel(int id, string name, string description, List<GuildMemberModel> members)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.members = members;
        }
    }
}
