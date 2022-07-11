using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    public class EquipementModel
    {
        public int id;
        public EquipementBaseModel equipementBase;
        public StatisticsModel statistics;

        public EquipementModel(int id, EquipementBaseModel equipementBase, StatisticsModel statistics)
        {
            this.id = id;
            this.equipementBase = equipementBase;
            this.statistics = statistics;
        }
    }
}
