using Lockstep.Serialization;

namespace Lockstep.Game {
    public partial class BaseComponent : IBackup {
        public virtual void Serialize(Serializer writer){ }
        public virtual void Deserialize(Deserializer reader){ }
        public virtual void OnAfterDeserialize(){ }
    }
}