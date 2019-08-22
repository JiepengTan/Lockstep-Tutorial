using Lockstep.Collision2D;
using Lockstep.Logic;
using Lockstep.Math;
using Lockstep.Util;
using UnityEngine;

namespace LockstepTutorial {
    public class UnityEntityService {
        public static GameObject CreateEntity(BaseEntity entity, int prefabId, LVector3 position, GameObject prefab,
            EntityConfig config){
            var obj = (GameObject) GameObject.Instantiate(prefab, position.ToVector3(), Quaternion.identity);
            entity.engineTransform = obj.transform;
            entity.transform.Pos3 = position;
            config.CopyTo(entity);
            entity.PrefabId = prefabId;
            CollisionManager.Instance.RegisterEntity(prefab, obj, entity);
            entity.DoAwake();
            entity.DoStart();
            var views = obj.GetComponents<IView>();
            foreach (var view in views) {
                view.BindEntity(entity);
            }
            return obj;
        }
    }
}