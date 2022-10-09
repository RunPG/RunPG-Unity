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
        public int strength;
        public int defense;
        public int power;
        public int resistance;
        public int precision;


        public StatisticsModel(int id, int level, int vitality, int strength, int defense, int power, int resistance, int precision)
        {
            this.id = id;
            this.level = level;
            this.vitality = vitality;
            this.strength = strength;
            this.defense = defense;
            this.power = power;
            this.resistance = resistance;
            this.precision = precision;
        }
    }
}
