using System;
using Lockstep.Game;

namespace Lockstep.UnityExt {
    public class HideInInspector : Attribute { }
}

namespace Lockstep.Collision2D {
    public partial class ColliderProxy {
        public Entity Entity => (Entity) EntityObject;
    }
}