using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunPG.Multi
{
    public class NewUserModel
    {
        public string name;
        public string uid;
        public string heroClass;

        public NewUserModel(string name, string uid, string heroClass)
        {
            this.name = name;
            this.uid = uid;
            this.heroClass = heroClass;
        }
    }
}
