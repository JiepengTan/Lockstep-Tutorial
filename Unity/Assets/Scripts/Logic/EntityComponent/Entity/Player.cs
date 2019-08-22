using System;
using System.Collections.Generic;
using Lockstep;
using Lockstep.Logic;
using Lockstep.Math;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LockstepTutorial {
    [Serializable]
    public partial class Player : Entity {
        public int localId;
        public PlayerInput input = new PlayerInput();
        public CMover mover = new CMover();

        public Player(){
            RegisterComponent(mover);
        }
        public override void DoUpdate(LFloat deltaTime){
            base.DoUpdate(deltaTime);
            if (input.skillId != -1) {
                Fire(input.skillId);
            }
        }
    }
}