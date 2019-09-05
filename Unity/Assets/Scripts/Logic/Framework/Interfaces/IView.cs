namespace Lockstep.Game {
    public interface IView {
        void BindEntity(BaseEntity e,BaseEntity oldEntity = null);
    }
}