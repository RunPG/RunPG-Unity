using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    public class Equipement
    {
        public int id;
        public int equipementBaseId;
        public int statisticsId;

        public Equipement(int id, int equipementBaseId, int statisticsId)
        {
            this.id = id;
            this.equipementBaseId = equipementBaseId;
            this.statisticsId = statisticsId;
        }
    }
}
