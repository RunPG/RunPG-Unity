using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunPG.Multi
{
    public class PlayerEquipmentModel
    {
        public int helmetId;
        public int chestplateId;
        public int glovesId;
        public int leggingsId;
        public int weaponId;

        public PlayerEquipmentModel(int helmetId, int chestplateId, int glovesId, int leggingsId, int weaponId)
        {
            this.helmetId = helmetId;
            this.chestplateId = chestplateId;
            this.glovesId = glovesId;
            this.leggingsId = leggingsId;
            this.weaponId = weaponId;
        }
    }
}
