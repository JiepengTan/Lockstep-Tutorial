using Lockstep.Game;

namespace Lockstep.Game {
    public partial class Enemy : IAfterBackup {public void OnAfterDeserialize(){}}
    public partial class Player : IAfterBackup {public void OnAfterDeserialize(){ }}
    public partial class Spawner : IAfterBackup {public void OnAfterDeserialize(){ }}

}