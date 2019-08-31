using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;

namespace LockstepTutorial{    
    public enum EColliderLayer {
        Static,
        Enemy,
        Hero,
        EnumCount
    }
    public class HeroManager : BaseLogicManager {
        public static HeroManager Instance;
        public override void DoStart(){ }
        public static GameObject InstantiateEntity(Player entity, int prefabId, LVector3 position){
            var prefab = ResourceGameService.LoadPrefab(prefabId);
            var config = ResourceGameService.GetPlayerConfig(prefabId);
            var obj = UnityEntityService.CreateEntity(entity, prefabId, position, prefab, config);
            return obj;
        }


        public override void DoUpdate(LFloat deltaTime){
            foreach (var player in Simulator.allPlayers) {
                player.DoUpdate(deltaTime);
            }
        }
    }
}