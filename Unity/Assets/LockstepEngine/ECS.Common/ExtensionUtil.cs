using System;
using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;


namespace Lockstep.ECS {
    public static class ExtensionUtil {
        public static void CopyTo(this object srcEntity, object destEntity, int componentIndex){
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

    }
}