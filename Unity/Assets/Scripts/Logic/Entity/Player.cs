using System.Collections.Generic;
using Lockstep;
using Lockstep.Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LockstepTutorial {
    public partial class Player : BaseEntity {
        public IPlayerView eventHandler;
        public PlayerInput InputAgent = new PlayerInput();
        public CMover CMover = new CMover();
        public int localId;

        public Player(){
            RegisterComponent(CMover);
        }

    }
}