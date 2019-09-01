using Lockstep.Serialization;

namespace Lockstep.Game {
    public interface IBackup  : ISerializable {
        void OnAfterDeserialize();//反序列化后重构
    }
}