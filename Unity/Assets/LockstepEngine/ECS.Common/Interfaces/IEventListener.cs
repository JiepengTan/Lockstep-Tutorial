

namespace Lockstep.Game {
    public interface IEventListener {
        void RegisterListeners(IEntity entity);
        void UnRegisterListeners();
    }
}
