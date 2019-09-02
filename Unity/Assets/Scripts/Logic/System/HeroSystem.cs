using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game{    
    public enum EColliderLayer {
        Static,
        Enemy,
        Hero,
        EnumCount
    }
    public class HeroSystem : BaseSystem {
        public override void DoUpdate(LFloat deltaTime){
            foreach (var player in _gameStateService.GetPlayers()) {
                player.DoUpdate(deltaTime);
            }
        }
    }
}