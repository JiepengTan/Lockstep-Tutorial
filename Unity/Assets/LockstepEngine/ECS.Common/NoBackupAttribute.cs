using System;

namespace Lockstep.Game {
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