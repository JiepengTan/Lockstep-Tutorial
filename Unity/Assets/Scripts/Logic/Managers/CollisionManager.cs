using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep.Logic;
using Lockstep.Collision2D;
using Lockstep.Math;
using Lockstep.UnsafeCollision2D;
using LockstepTutorial;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;
using Ray2D = Lockstep.Collision2D.Ray2D;
using Debug = Lockstep.Logging.Debug;

namespace LockstepTutorial {
    public class CollisionManager : UnityBaseManager {
        public Vector2 scrollPos;
        public bool isShow = true;
        public bool[] collisionMatrix = new bool[(int) EColliderLayer.EnumCount * (int) EColliderLayer.EnumCount];
        private static CollisionManager _instance;

        private string[] _colliderLayerNames;

        public string[] ColliderLayerNames {
            get {
                if (_colliderLayerNames == null || _colliderLayerNames.Length == 0) {
                    var lst = new List<string>();
                    for (int i = 0; i < (int) EColliderLayer.EnumCount; i++) {
                        lst.Add(((EColliderLayer) i).ToString());
                    }

                    _colliderLayerNames = lst.ToArray();
                }

                return _colliderLayerNames;
            }
        }

        public void SetColliderPair(int a, int b, bool val){
            collisionMatrix[a * (int) EColliderLayer.EnumCount + b] = val;
            collisionMatrix[b * (int) EColliderLayer.EnumCount + a] = val;
        }

        public bool GetColliderPair(int a, int b){
            return collisionMatrix[a * (int) EColliderLayer.EnumCount + b];
        }

        public static CollisionManager Instance => _instance;

        static Dictionary<GameObject, ColliderPrefab> _go2ColPrefab = new Dictionary<GameObject, ColliderPrefab>();
        static Dictionary<GameObject, int> _go2Layer = new Dictionary<GameObject, int>();

        static Dictionary<ILPTriggerEventHandler, ColliderProxy> _mono2ColProxy =
            new Dictionary<ILPTriggerEventHandler, ColliderProxy>();

        static Dictionary<ColliderProxy, ILPTriggerEventHandler> _colProxy2Mono =
            new Dictionary<ColliderProxy, ILPTriggerEventHandler>();

        ICollisionSystem collisionSystem;

        public LVector3 pos;
        public LFloat worldSize = new LFloat(60);
        public LFloat minNodeSize = new LFloat(1);
        public LFloat loosenessval = new LFloat(true, 1250);

        public LFloat percent = new LFloat(true, 100);
        public int count = 100;


        private LFloat halfworldSize => worldSize / 2 - 5;

        private int[] allTypes = new int[] {0, 1, 2};

        public int showTreeId = 0;

        public override void DoAwake(){
            _instance = this;
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
            Debug.Trace($"worldSize:{worldSize} pos:{pos} minNodeSize:{minNodeSize} loosenessval:{loosenessval}");
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


        public void RigisterPrefab(GameObject go, int val){
            _go2Layer[go] = val;
        }

        public void RegisterEntity(GameObject fab, GameObject obj, BaseEntity entity){
            ColliderPrefab prefab = null;
            if (!_go2ColPrefab.TryGetValue(fab, out prefab)) {
                prefab = CollisionSystem.CreateColliderPrefab(fab, entity.colliderData);
            }

            AttachToColSystem(_go2Layer[fab], prefab, obj, entity);
        }

        public void AttachToColSystem(int layer, ColliderPrefab prefab, GameObject obj, BaseEntity entity){
            var proxy = new ColliderProxy();
            proxy.EntityObject = entity;
            proxy.Init(prefab, entity.transform);
#if UNITY_EDITOR
            proxy.UnityTransform = obj.transform;
#endif
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