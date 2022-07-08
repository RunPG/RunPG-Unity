using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RunPG.Multi
{
    public class EquipementBaseModel
    {
        
        public int id;
        public string name;
        public string description;
        public Rarity rarity;
        public string heroClass;
        public EquipementType equipementType;

        public EquipementBaseModel(int id, string name, string description, Rarity rarity, string heroClass, EquipementType equipementType)
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
