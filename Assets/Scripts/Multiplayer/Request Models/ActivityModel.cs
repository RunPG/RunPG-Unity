using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunPG.Multi
{
    public class ActivityModel
    {
        public long lastAccess;

        public ActivityModel(long lastAccess)
        {
            this.lastAccess = lastAccess;
        }
    }
}
