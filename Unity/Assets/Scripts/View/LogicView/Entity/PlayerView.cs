using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game {
    public class PlayerView : EntityView, IPlayerView {
        public Player Player;
        protected bool isDead => entity?.isDead ?? true;

        public override void BindEntity(BaseEntity e, BaseEntity oldEntity = null){
            base.BindEntity(e,oldEntity);
            Player = e as Player;
        }
    }
}