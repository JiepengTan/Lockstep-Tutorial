using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public class AnimatorView : MonoBehaviour, IAnimatorView {
        public Animation animComp;
        public Transform rootTrans;
        public AnimationState animState;
        private CAnimator cAnim;
        private Animator anim;
        public LFloat speed;

        void Start(){
            if (animComp == null) {
                animComp = GetComponent<Animation>();
                if (animComp == null) {
                    animComp = GetComponentInChildren<Animation>();
                }
            }
        }

        public void SetInteger(string name, int val){
            anim.SetInteger(name, val);
        }

        public void SetTrigger(string name){
            anim.SetTrigger(name);
        }

        public void Play(string name, bool isCross){
            animState = animComp[name];
            var state = animComp[name];
            if (state != null) {
                if (isCross) {
                    animComp.CrossFade(name);
                }
                else {
                    animComp.Play(name);
                }
            }
        }

        public void LateUpdate(){
            if (cAnim.curAnimBindInfo != null && cAnim.curAnimBindInfo.isMoveByAnim) {
                rootTrans.localPosition = Vector3.zero;
            }
        }

        public void Sample(LFloat time){
            if (Application.isPlaying) {
                return;
            }

            if (animState == null) return;
            if (!Application.isPlaying) {
                animComp.Play();
            }

            animState.enabled = true;
            animState.weight = 1;
            animState.time = time.ToFloat();
            animComp.Sample();
            if (!Application.isPlaying) {
                animState.enabled = false;
            }
        }
    }
}