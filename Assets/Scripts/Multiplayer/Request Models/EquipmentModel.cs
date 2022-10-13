using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    public class EquipmentModel
    {
        public int id;
        public EquipmentBaseModel equipementBase; // TODO rename
        public StatisticsModel statistics;

        public EquipmentModel(int id, EquipmentBaseModel equipmentBase, StatisticsModel statistics)
        {
            this.id = id;
            this.equipementBase = equipmentBase;
            this.statistics = statistics;
        }
    }
}
