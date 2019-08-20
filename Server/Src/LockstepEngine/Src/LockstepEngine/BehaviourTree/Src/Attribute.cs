using System;

namespace Lockstep.BehaviourTree {
   
    [AttributeUsage(AttributeTargets.Class,Inherited = false)]
    public class StringInfoAttribute : Attribute {
        public int idx = 0;
        public string menu;

        public StringInfoAttribute(string menu,int idx){
            this.menu = menu;
            this.idx = idx;
        }
    }

    [AttributeUsage(AttributeTargets.Class,Inherited = false)]
    public class BuildInNodeAttribute : StringInfoAttribute {
        public BuildInNodeAttribute(string menu,EBTBuildInTypeIdx eidx) : base(menu,(int) eidx){ }

        public BuildInNodeAttribute(Type type,EBTBuildInTypeIdx eidx): base(type.Name,(int) eidx){
        }
    }
}