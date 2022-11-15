using Assets.Scripts.Multiplayer.Request_Models;
using System;
using System.Collections.Generic;

namespace RunPG.Multi
{
    public class GuildListModel
    {
        public int id;
        public string name;
        public string description;

        public GuildListModel(int id, string name, string description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
        }
    }
}
