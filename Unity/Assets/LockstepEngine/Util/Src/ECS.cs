namespace Lockstep {
    public partial interface IEntity:INeedBackup {}
    public partial interface IContexts { }
    public partial interface IComponent :INeedBackup{ }
    public partial interface INeedBackup { }
}