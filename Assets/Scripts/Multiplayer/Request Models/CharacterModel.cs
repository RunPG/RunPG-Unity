using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    public enum HeroClass
    {
        MAGE,
        BERSERKER,
        PRIEST,
        ROGUE,
        PALADIN
    }
    public class CharacterModel
    {
        public int id;
        public int experience;
        public int statisticsId;
        public int firstSpellId;
        public int secondSepllId;
        public int thirdSpellId;
        public int fourthSepllId;
        public int helmetId;
        public int chestplateId;
        public int leggingsId;
        public int glovesId;
        public int weapondId;
        public HeroClass heroClass;

        public CharacterModel(int id, int experience, int statisticsId, int firstSpellId, int secondSepllId, int thirdSpellId, int fourthSepllId, int helmetId, int chestplateId, int leggingsId, int glovesId, int weapondId, HeroClass heroClass)
        {
            this.id = id;
            this.experience = experience;
            this.statisticsId = statisticsId;
            this.firstSpellId = firstSpellId;
            this.secondSepllId = secondSepllId;
            this.thirdSpellId = thirdSpellId;
            this.fourthSepllId = fourthSepllId;
            this.helmetId = helmetId;
            this.chestplateId = chestplateId;
            this.leggingsId = leggingsId;
            this.glovesId = glovesId;
            this.weapondId = weapondId;
            this.heroClass = heroClass;
    }
}
}
