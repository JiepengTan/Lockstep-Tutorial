using UnityEngine;

namespace Lockstep.Game {
    [PureMode(EPureModeType.Unity)]
    public abstract class UnityBaseService : BaseService {
        public Transform transform { get; protected set; }
        public GameObject gameObject { get; protected set; }
        public override void DoInit(object objParent){
            var parent = objParent as Transform;
            base.DoInit(parent);
            InitGameObject(parent);
        }
        
        
        private void InitGameObject(Transform parent){
            var go = new GameObject(GetType().Name);
            gameObject = go;
            transform = go.transform;
            transform.SetParent(parent, false);
        }

        public override void DoDestroy(){
            if (gameObject != null) {
                if (Application.isPlaying) {
                    GameObject.Destroy(gameObject);
                }
                else {
                    GameObject.DestroyImmediate(gameObject);
                }
            }
        }
    }
}