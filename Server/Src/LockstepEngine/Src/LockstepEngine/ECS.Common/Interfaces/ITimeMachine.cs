using System.Collections;
using System.Collections.Generic;

namespace Lockstep.Game {
    public interface ITimeMachine {
        int CurTick { get; set; }
        ///Rollback to tick , so all cmd between [tick,~)(Include tick) should undo
        void RollbackTo(int tick);
        void Backup(int tick);
        ///Discard all cmd between [0,maxVerifiedTick] (Include maxVerifiedTick)
        void Clean(int maxVerifiedTick);
    }
}