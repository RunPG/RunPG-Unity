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
        public HeroClass heroClass;
        public EquipmentType equipmentType;

        public EquipmentBaseModel(int id, string name, string description, Rarity rarity, HeroClass heroClass, EquipmentType equipmentType)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.rarity = rarity;
            this.heroClass = heroClass;
            this.equipmentType = equipmentType;
        }
    }
}
