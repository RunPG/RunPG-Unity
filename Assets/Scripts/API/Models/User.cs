using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    internal class User
    {
        public int id;
        public string name;
        public int id_guild;
        public int id_character;

        public User(int id, string name, int id_guild, int id_character)
        {
            this.id = id;
            this.name = name;
            this.id_guild = id_guild;
            this.id_character = id_character;
        }
    }

}
