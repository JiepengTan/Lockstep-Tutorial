using System;

namespace Lockstep.ECS.ECDefine {
    [AttributeUsage(AttributeTargets.Method)]
    public class SignalAttribute : System.Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class AbstractAttribute : System.Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class EntityCountAttribute : System.Attribute {
        public int count;

        public EntityCountAttribute(int count){
            this.count = count;
        }
    }

    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Field,AllowMultiple = true)]
    public class AttributeAttribute : System.Attribute {
        public string name;
        public AttributeAttribute(string name){
            this.name = name;
        }
    }
    
    [AttributeUsage(AttributeTargets.Enum)]
    public class IgnoreAttribute : System.Attribute {
    }
}