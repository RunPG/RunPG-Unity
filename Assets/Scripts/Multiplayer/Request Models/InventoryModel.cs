using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunPG.Multi
{
    public class InventoryModel
    {
        public int id;
        public int userId;
        public int stackSize;
        public int? equipmentId;
        public int? itemId;

        public InventoryModel(int id, int userId, int stackSize, int? equipmentId, int? itemId)
        {
            this.id = id;
            this.userId = userId;
            this.stackSize = stackSize;
            this.equipmentId = equipmentId;
            this.itemId = itemId;
        }
    }
}