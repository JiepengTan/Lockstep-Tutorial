using Lockstep.UnsafeCollision2D;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Collision2D {
    
    public class ColliderProxyMono : MonoBehaviour {
        public ColliderProxy proxy;

        private Material mat;
        private bool hasCollided = false;
        public Color rawColor = Color.yellow;
        private Renderer render;
        private Material rawmat;
        public static bool IsReplaceNullMat;

        private void Start(){
            render = GetComponent<SkinnedMeshRenderer>();
            if (render == null) {
                render = GetComponentInChildren<SkinnedMeshRenderer>();
            }

            if (render == null) {
                render = GetComponent<Renderer>();
                if (render == null) {
                    render = GetComponentInChildren<Renderer>();
                }
            }

            if (render != null) {
                rawmat = render.material;
                mat = new Material(render.material);
                render.material = mat;
            }

        }

        public void Update(){
            if (proxy == null) return;
            
            //if (mat != null) {
            //    mat.color = hasCollided ? rawColor * 0.2f : rawColor;
            //    if (IsReplaceNullMat) {
            //        render.material = hasCollided ? null : rawmat;
            //    }
            //}

            if (hasCollided) {
                hasCollided = false;
            }
        }

        public bool IsDebug = false;

        void OnTriggerEvent(ColliderProxy other, ECollisionEvent type){
            hasCollided = true;
            if (IsDebug) {
                if (type != ECollisionEvent.Stay) {
                    Debug.Log(type);
                }
            }

            if (proxy.IsStatic) {
                int i = 0;
            }
        }
    }
}