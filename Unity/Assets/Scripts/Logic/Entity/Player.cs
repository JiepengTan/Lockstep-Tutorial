using System.Collections.Generic;
using Lockstep;
using Lockstep.Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LockstepTutorial {
    public partial class Player : BaseActor {
        public PlayerInput InputAgent = new PlayerInput();
        public CMover CMover = new CMover();
        public int localId;

        public Player(){
            RegisterComponent(CMover);
        }

    }
}