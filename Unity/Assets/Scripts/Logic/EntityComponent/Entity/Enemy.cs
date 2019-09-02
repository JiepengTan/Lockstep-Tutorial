using System;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    [Serializable]
    public partial class Enemy : Entity {
        public CBrain brain = new CBrain();

        public Enemy(){
            moveSpd = 2;
            turnSpd = 150;
            RegisterComponent(brain);
        }

    }
}