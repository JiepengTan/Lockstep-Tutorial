using Lockstep.Logic;
using Lockstep.Math;
using UnityEngine;
using UnityEngine.UI;

namespace LockstepTutorial {
    public partial class PlayerView : MonoBehaviour, IPlayerView {
        public Player owner;
        public int currentHealth => owner.currentHealth;

        public static LFloat MinRunSpd = new LFloat(1);
        public static LFloat MinFastRunSpd = new LFloat(7);

        public void Animating(bool isIdle){
        }

        public void BindEntity(BaseEntity entity){
            owner = entity as Player;
            var config = ResourceManager.GetPlayerConfig(owner.PrefabId);
            var go = transform.Find(config.attackTransName);
            //init shooting
            transform.position = owner.transform.Pos3.ToVector3();

            owner.eventHandler = this;
        }

        void Awake(){
        }

        private void Update(){
            var pos = owner.transform.Pos3.ToVector3();
            transform.position = Vector3.Lerp(transform.position, pos, 0.3f);
            var deg = owner.transform.deg.ToFloat();
            //deg = Mathf.Lerp(transform.rotation.eulerAngles.y, deg, 0.3f);
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0, deg, 0),0.3f) ;
        }

        public void TakeDamage(int amount, LVector3 hitPoint){}
        public void OnDead(){DisableEffects();}
        public void DisableEffects(){}
    }
}