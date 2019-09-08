using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Collision2D;
using Lockstep.Math;
using Lockstep.UnsafeCollision2D;
using Lockstep.Game;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;
using Ray2D = Lockstep.Collision2D.Ray2D;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    public class PhysicSystem : BaseSystem {
        private static PhysicSystem _instance;
        public static PhysicSystem Instance => _instance;
        ICollisionSystem collisionSystem;
        public CollisionConfig config;

        static Dictionary<int, ColliderPrefab> _fabId2ColPrefab = new Dictionary<int, ColliderPrefab>();
        static Dictionary<int, int> _fabId2Layer = new Dictionary<int, int>();

        static Dictionary<ILPTriggerEventHandler, ColliderProxy> _mono2ColProxy =
            new Dictionary<ILPTriggerEventHandler, ColliderProxy>();

        static Dictionary<ColliderProxy, ILPTriggerEventHandler> _colProxy2Mono =
            new Dictionary<ColliderProxy, ILPTriggerEventHandler>();

        public bool[] collisionMatrix => config.collisionMatrix;
        public LVector3 pos => config.pos;
        public LFloat worldSize => config.worldSize;
        public LFloat minNodeSize => config.minNodeSize;
        public LFloat loosenessval => config.loosenessval;

        private LFloat halfworldSize => worldSize / 2 - 5;

        private int[] allTypes = new int[] {0, 1, 2};

        public int showTreeId = 0;

        public override void DoAwake(IServiceContainer services){
            _instance = this;
            config = _gameConfigService.CollisionConfig;
            DoStart();
        }

        public override void DoStart(){
            if (_instance != this) {
                Debug.LogError("Duplicate CollisionSystemAdapt!");
                return;
            }

            var collisionSystem = new CollisionSystem() {
                worldSize = worldSize,
                pos = pos,
                minNodeSize = minNodeSize,
                loosenessval = loosenessval
            };
            _debugService.Trace($"worldSize:{worldSize} pos:{pos} minNodeSize:{minNodeSize} loosenessval:{loosenessval}");
            this.collisionSystem = collisionSystem;
            collisionSystem.DoStart(collisionMatrix, allTypes);
            collisionSystem.funcGlobalOnTriggerEvent += GlobalOnTriggerEvent;
        }

        public override void DoUpdate(LFloat deltaTime){
            collisionSystem.ShowTreeId = showTreeId;
            collisionSystem.DoUpdate(deltaTime);
        }

        public static void GlobalOnTriggerEvent(ColliderProxy a, ColliderProxy b, ECollisionEvent type){
            if (_colProxy2Mono.TryGetValue(a, out var handlera)) {
                CollisionSystem.TriggerEvent(handlera, b, type);
            }

            if (_colProxy2Mono.TryGetValue(b, out var handlerb)) {
                CollisionSystem.TriggerEvent(handlerb, a, type);
            }
        }


        public static ColliderProxy GetCollider(int id){
            return _instance.collisionSystem.GetCollider(id);
        }

        public static bool Raycast(int layerMask, Ray2D ray, out LRaycastHit2D ret){
            return Raycast(layerMask, ray, out ret, LFloat.MaxValue);
        }

        public static bool Raycast(int layerMask, Ray2D ray, out LRaycastHit2D ret, LFloat maxDistance){
            ret = new LRaycastHit2D();
            LFloat t = LFloat.one; 
            int id;
            if (_instance.DoRaycast(layerMask, ray, out t, out id, maxDistance)) {
                ret.point = ray.origin + ray.direction * t;
                ret.distance = t * ray.direction.magnitude;
                ret.colliderId = id;
                return true;
            }

            return false;
        }

        public static void QueryRegion(int layerType, LVector2 pos, LVector2 size, LVector2 forward,
            FuncCollision callback){
            _instance._QueryRegion(layerType, pos, size, forward, callback);
        }

        public static void QueryRegion(int layerType, LVector2 pos, LFloat radius, FuncCollision callback){
            _instance._QueryRegion(layerType, pos, radius, callback);
        }

        private void _QueryRegion(int layerType, LVector2 pos, LVector2 size, LVector2 forward, FuncCollision callback){
            collisionSystem.QueryRegion(layerType, pos, size, forward, callback);
        }

        private void _QueryRegion(int layerType, LVector2 pos, LFloat radius, FuncCollision callback){
            collisionSystem.QueryRegion(layerType, pos, radius, callback);
        }

        public bool DoRaycast(int layerMask, Ray2D ray, out LFloat t, out int id, LFloat maxDistance){
            Profiler.BeginSample("DoRaycast ");
            var ret = collisionSystem.Raycast(layerMask, ray, out t, out id, maxDistance);
            Profiler.EndSample();
            return ret;
        }


        public void RigisterPrefab(int prefabId, int val){
            _fabId2Layer[prefabId] = val;
        }

        public void RegisterEntity(int prefabId, Entity entity){
            ColliderPrefab prefab = null;
            var fab = _gameResourceService.LoadPrefab(prefabId) as GameObject;
            if (!_fabId2ColPrefab.TryGetValue(prefabId, out prefab)) {
                prefab = CollisionSystem.CreateColliderPrefab(fab, entity.colliderData);
            }

            AttachToColSystem(_fabId2Layer[prefabId], prefab,  entity);
        }

        public void AttachToColSystem(int layer, ColliderPrefab prefab, BaseEntity entity){
            var proxy = new ColliderProxy();
            proxy.EntityObject = entity;
            proxy.Init(prefab, entity.transform);
            proxy.IsStatic = false;
            proxy.LayerType = layer;
            var eventHandler = entity;
            if (eventHandler != null) {
                _mono2ColProxy[eventHandler] = proxy;
                _colProxy2Mono[proxy] = eventHandler;
            }

            collisionSystem.AddCollider(proxy);
        }

        public void RemoveCollider(ILPTriggerEventHandler handler){
            if (_mono2ColProxy.TryGetValue(handler, out var proxy)) {
                RemoveCollider(proxy);
                _mono2ColProxy.Remove(handler);
                _colProxy2Mono.Remove(proxy);
            }
        }

        public void RemoveCollider(ColliderProxy collider){
            collisionSystem.RemoveCollider(collider);
        }

        void OnDrawGizmos(){
            collisionSystem?.DrawGizmos();
        }
    }
}