using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RunPG.Multi
{
    public class EquipmentBaseModel
    {
        
        public int id;
        public string name;
        public string description;
        public Rarity rarity;
        public string heroClass;
        public EquipmentType equipementType; // TODO rename

        public EquipmentBaseModel(int id, string name, string description, Rarity rarity, string heroClass, EquipmentType equipementType)
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
