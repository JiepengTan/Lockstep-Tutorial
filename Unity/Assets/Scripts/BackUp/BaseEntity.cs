using System;
using Lockstep.Collision2D;
using Lockstep.Game;
using Lockstep.Math;
using Lockstep.Serialization;

namespace Lockstep.Game {
    public partial class BaseEntity : IBackup {
        public virtual void Serialize(Serializer writer){ }
        public virtual void Deserialize(Deserializer reader){ }
        public virtual void OnAfterDeserialize(){ }
    }
}