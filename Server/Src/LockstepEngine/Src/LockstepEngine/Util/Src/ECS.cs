using System;

namespace Lockstep {
    public partial interface IEntity:INeedBackup {}
    public partial interface IContexts { }
    public partial interface IComponent :INeedBackup{ }
    public partial interface INeedBackup { }
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false,
        Inherited = true)]
    public class NoBackupAttribute : Attribute { }
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false,
        Inherited = true)]
    public class BackupAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false,
        Inherited = true)]
    public class ReRefBackupAttribute : Attribute { }
}