using Lockstep.Logic;
using Lockstep.Math;
using Lockstep.Util;
using UnityEngine;

namespace LockstepTutorial {
    public class UnityEntityService {
        public static GameObject CreateEntity(BaseEntity entity, int prefabId, LVector3 position, GameObject prefab,
            object config){
            var obj = (GameObject) GameObject.Instantiate(prefab, position.ToVector3(), Quaternion.identity);
            entity.engineTransform = obj.transform;
            entity.transform.Pos3 = position;
            config.CopyFiledsTo(entity);
            var views = obj.GetComponents<IView>();
            foreach (var view in views) {
                view.BindEntity(entity);
            }
            entity.PrefabId = prefabId;
            entity.DoAwake();
            entity.DoStart();
            return obj;
        }
    }
}