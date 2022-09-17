using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunPG.Multi
{
    public class NewUserModel
    {
        public string name;
        public string uid;
        public string mail;
        public string serverSideAccessCode;
        public string heroClass;

        public NewUserModel(string name, string uid, string mail, string serverSideAccessCode, string heroClass)
        {
            this.name = name;
            this.uid = uid;
            this.mail = mail;
            this.serverSideAccessCode = serverSideAccessCode;
            this.heroClass = heroClass;
        }
    }
}
