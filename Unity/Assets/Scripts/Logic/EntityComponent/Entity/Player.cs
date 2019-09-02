using System;
using System.Collections.Generic;
using Lockstep;
using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lockstep.Game {
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