using System;
using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;


namespace Lockstep.ECS {
    public static class ExtensionUtil {
        public static void CopyTo(this Entity srcEntity, Entity destEntity, int componentIndex){
#if false
            var component1 = srcEntity.GetComponent(componentIndex);
            Stack<IComponent> componentPool = destEntity.GetComponentPool(componentIndex);
            IComponent backupComponent = null;
            if (componentPool.Count <= 0) {
                if (component1 is ICloneable cloneComp) {
                    backupComponent = cloneComp.Clone() as IComponent;
                }
                else 
                {
                    backupComponent = (IComponent) Activator.CreateInstance(component1.GetType());
                    component1.CopyPublicMemberValues(backupComponent);
                }
            }
            else {
                backupComponent = componentPool.Pop();
                if (component1 is ICloneable cloneComp) {
                    cloneComp.CopyTo(backupComponent);
                }
                else 
                {
                    component1.CopyPublicMemberValues(backupComponent);
                }
            }
            destEntity.AddComponent(componentIndex, backupComponent);
#endif
        }
    }
}

namespace Lockstep.Serialization {
    public static class ExtensionSerializer {
        public static void Write(this Serializer serializer, LFloat val){
            serializer.Write(val._val);
        }

        public static void Write(this Serializer serializer, LVector2 val){
            serializer.Write(val._x);
            serializer.Write(val._y);
        }

        public static void Write(this Serializer serializer, LVector3 val){
            serializer.Write(val._x);
            serializer.Write(val._y);
            serializer.Write(val._z);
        }


        public static LFloat ReadLFloat(this Deserializer reader){
            var x = reader.ReadInt32();
            return new LFloat(true, x);
        }

        public static LVector2 ReadLVector2(this Deserializer reader){
            var x = reader.ReadInt32();
            var y = reader.ReadInt32();
            return new LVector2(true, x, y);
        }

        public static LVector3 ReadLVector3(this Deserializer reader){
            var x = reader.ReadInt32();
            var y = reader.ReadInt32();
            var z = reader.ReadInt32();
            return new LVector3(true, x, y, z);
        }
    }
}