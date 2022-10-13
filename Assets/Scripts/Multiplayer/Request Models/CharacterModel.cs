using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RunPG.Multi
{
    public class CharacterModel
    {
        public int id;
        public int experience;
        public int statisticsId;
        public int firstSpellId;
        public int secondSpellId;
        public int thirdSpellId;
        public int fourthSpellId;
        public int helmetId;
        public int chestplateId;
        public int leggingsId;
        public int glovesId;
        public int weaponId;
        public HeroClass heroClass;

        public CharacterModel(int id, int experience, int statisticsId, int firstSpellId, int secondSpellId, int thirdSpellId, int fourthSpellId, int helmetId, int chestplateId, int leggingsId, int glovesId, int weaponId, HeroClass heroClass)
        {
            this.id = id;
            this.experience = experience;
            this.statisticsId = statisticsId;
            this.firstSpellId = firstSpellId;
            this.secondSpellId = secondSpellId;
            this.thirdSpellId = thirdSpellId;
            this.fourthSpellId = fourthSpellId;
            this.helmetId = helmetId;
            this.chestplateId = chestplateId;
            this.leggingsId = leggingsId;
            this.glovesId = glovesId;
            this.weaponId = weaponId;
            this.heroClass = heroClass;
        }
    }
}
