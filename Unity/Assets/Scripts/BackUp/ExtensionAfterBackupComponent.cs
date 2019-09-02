using Lockstep.Game;

namespace Lockstep.Game {
    public partial class CAnimator     : IAfterBackup{ public void OnAfterDeserialize(){ }    }
    public partial class CBrain        : IAfterBackup{ public void OnAfterDeserialize(){ }    }
    public partial class CMover        : IAfterBackup{ public void OnAfterDeserialize(){ }    }
    public partial class CRigidbody : IAfterBackup {public void OnAfterDeserialize(){ }}
    public partial class PlayerInput   : IAfterBackup{ public void OnAfterDeserialize(){ }    }
    public partial class Skill         : IAfterBackup{ public void OnAfterDeserialize(){ }    }
    public partial class SpawnerInfo    : IAfterBackup{public void OnAfterDeserialize(){ }     }
}

namespace Lockstep.Collision2D {
    public partial class ColliderData  : IAfterBackup{ public void OnAfterDeserialize(){ }    }
    public partial class CTransform2D : IAfterBackup {public void OnAfterDeserialize(){ }}
}