using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    public class Equipement
    {
        public int id;
        public string name;
        public string description;
        public string rarity;
        public string heroClass;
        public string equipementType;

        public Equipement(int id, string name, string description, string rarity, string heroClass, string equipementType)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.rarity = rarity;
            this.heroClass = heroClass;
            this.equipementType = equipementType;
        }
    }
}
