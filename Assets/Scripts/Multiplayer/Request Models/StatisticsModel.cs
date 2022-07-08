using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    public class StatisticsModel
    {
        public int id;
        public int level;
        public int vitality;
        public int endurance;
        public int strength;
        public int defense;
        public int power;
        public int resistance;
        public int precision;
        public int agility;


        public StatisticsModel(int id, int level, int vitality, int endurance, int strength, int defense, int power, int resistance, int precision, int agility)
        {
            this.id = id;
            this.level = level;
            this.vitality = vitality;
            this.endurance = endurance;
            this.strength = strength;
            this.defense = defense;
            this.power = power;
            this.resistance = resistance;
            this.precision = precision;
            this.agility = agility;
        }
    }
}
