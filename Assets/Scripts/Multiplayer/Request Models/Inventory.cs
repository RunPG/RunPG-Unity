using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    public class Inventory
    {
        public int id;
        public int userId;
        public int stackSize;
        public int? equipementId;
        public int? itemId;

        public Inventory(int id, int userId, int stackSize, int? equipementId, int? itemId)
        {
            this.id = id;
            this.userId = userId;
            this.stackSize = stackSize;
            this.equipementId = equipementId;
            this.itemId = itemId;
        }
    }
}