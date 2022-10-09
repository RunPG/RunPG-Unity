using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunPG.Multi
{
    public class NewEquipementModel
    {
        public string equipementBaseId;
        public StatisticsModel statistics;

        public NewEquipementModel(string equipementBaseId, StatisticsModel statistics)
        {
            this.equipementBaseId = equipementBaseId;
            this.statistics = statistics;
        }
    }
}
