using System;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    [Serializable]
    public partial class Enemy : Entity {
        public CBrain brain = new CBrain();

        protected override void BindRef(){
            base.BindRef();
            RegisterComponent(brain);
            moveSpd = 2;
            turnSpd = 150;
        }
    }
}