using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    public class GuildCreateModel
    {
        public int ownerId;
        public string name;
        public string description;

        public GuildCreateModel(string name, string description, int ownerId)
        {
            this.ownerId = ownerId;
            this.name = name;
            this.description = description;
        }
    }
}
