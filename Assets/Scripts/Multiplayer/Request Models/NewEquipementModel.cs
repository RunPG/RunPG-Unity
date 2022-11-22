using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunPG.Multi
{
    public class NewEquipementModel
    {
        public int equipmentBaseId;
        public StatisticsModel statistics;

        public NewEquipementModel(int equipementBaseId, StatisticsModel statistics)
        {
            this.equipmentBaseId = equipementBaseId;
            this.statistics = statistics;
        }
    }
}
