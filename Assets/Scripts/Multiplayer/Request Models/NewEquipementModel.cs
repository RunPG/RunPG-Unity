using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunPG.Multi
{
    public class NewEquipementModel
    {
        public int equipementBaseId;
        public StatisticsModel statistics;

        public NewEquipementModel(int equipementBaseId, StatisticsModel statistics)
        {
            this.equipementBaseId = equipementBaseId;
            this.statistics = statistics;
        }
    }
}
