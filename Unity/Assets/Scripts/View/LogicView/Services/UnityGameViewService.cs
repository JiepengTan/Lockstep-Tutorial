using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public class UnityGameViewService : BaseGameService, IGameViewService {
        public void BindView(BaseEntity entity){
            var prefab = _gameResourceService.LoadPrefab(entity.PrefabId) as GameObject;
            if(prefab == null) return;
            var obj = (GameObject) GameObject.Instantiate(prefab,
                entity.transform.Pos3.ToVector3(),
                Quaternion.Euler(new Vector3(0, entity.transform.deg, 0)));
            entity.engineTransform = obj.transform;
            var views = obj.GetComponents<IView>();
            foreach (var view in views) {
                view.BindEntity(entity);
            }
        }

        public void UnbindView(BaseEntity entity){
            entity.OnRollbackDestroy();
        }
    }
}