namespace Lockstep.Network {
    public interface IMessageDispatcher {
        void Dispatch(Session session, Packet packet);
    }
}