
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using Lockstep.UnsafeCollision2D;
using Lockstep.Math;
using UnityEngine;
using UnityEngine.Profiling;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

namespace Lockstep.Collision2D {
    //Test 1000 count 
    //percent 0.4
    //world size 300
    //loosensess val 2.0f
    public class TestQuadTree : MonoBehaviour {
        public Vector3 pos;

        ICollisionSystem collisionSystem;
        public float worldSize = 150;
        public float minNodeSize = 1;
        public float loosenessval = 1.25f;

        private float halfworldSize => worldSize / 2 - 5;

        private List<ColliderPrefab> prefabs = new List<ColliderPrefab>();

        public float percent = 0.1f;
        public int count = 100;

        public bool[] InterestingMasks;

        private int[] allTypes = new int[] {0, 1, 2};

        private void Start(){
            // Initial size (metres), initial centre position, minimum node size (metres), looseness
            collisionSystem = new CollisionSystem() {
                worldSize = worldSize.ToLFloat(),
                pos = pos.ToLVector3(),
                minNodeSize = minNodeSize.ToLFloat(),
                loosenessval = loosenessval.ToLFloat()
            };
            collisionSystem.DoStart(InterestingMasks, allTypes);
            //init prefab 
            const int size = 4;

            void CreatePrefab(CBaseShape collider){
                var prefab = new ColliderPrefab();
                prefab.parts.Add(new ColliderPart() {
                    transform = new CTransform2D(LVector2.zero),
                    collider = collider
                });
                prefabs.Add(prefab);
            }

            for (int i = 1; i < size; i++) {
                for (int j = 1; j < size; j++) {
                    CreatePrefab(new CAABB(new LVector2(i, j)));
                    CreatePrefab(new COBB(new LVector2(i, j), LFloat.zero));
                    CreatePrefab(new CCircle(((i + j) * 0.5f).ToLFloat()));
                }
            }

            for (int i = 0; i < count; i++) {
                int layerType = 0;
                var rawColor = Color.white;
                bool isStatic = true;
                if (i < percent * count * 2) {
                    layerType = 1;
                    isStatic = false;
                    rawColor = Color.yellow;
                    if (i < percent * count) {
                        rawColor = Color.green;
                        layerType = 2;
                    }
                }

                var proxy = CreateType(layerType, isStatic, rawColor);
                collisionSystem.AddCollider(proxy);
            }
        }

        Dictionary<EShape2D, PrimitiveType> type2PType = new Dictionary<EShape2D, PrimitiveType>() {
            {EShape2D.Circle, PrimitiveType.Cylinder},
            {EShape2D.AABB, PrimitiveType.Cube},
            {EShape2D.OBB, PrimitiveType.Cube},
        };

        private ColliderProxy CreateType(int layerType, bool isStatic, Color rawColor){
            var prefab = prefabs[Random.Range(0, prefabs.Count)];
            var type = (EShape2D) prefab.collider.TypeId;
            var obj = GameObject.CreatePrimitive(type2PType[type]).GetComponent<Collider>();
            obj.transform.SetParent(transform, false);
            obj.transform.position = new Vector3(Random.Range(-halfworldSize, halfworldSize), 0,
                Random.Range(-halfworldSize, halfworldSize));
            switch (type) {
                case EShape2D.Circle: {
                    var colInfo = (CCircle) prefab.collider;
                    obj.transform.localScale =
                        new Vector3(colInfo.radius.ToFloat() * 2, 1, colInfo.radius.ToFloat() * 2);
                    break;
                }
                case EShape2D.AABB: {
                    var colInfo = (CAABB) prefab.collider;
                    obj.transform.localScale =
                        new Vector3(colInfo.size.x.ToFloat() * 2, 1, colInfo.size.y.ToFloat() * 2);
                    break;
                }
                case EShape2D.OBB: {
                    var colInfo = (COBB) prefab.collider;
                    obj.transform.localScale =
                        new Vector3(colInfo.size.x.ToFloat() * 2, 1, colInfo.size.y.ToFloat() * 2);
                    break;
                }
            }

            var proxy = new ColliderProxy();
            proxy.Init(prefab, obj.transform.position.ToLVector2XZ());
            if (!isStatic) {
                var mover = obj.gameObject.AddComponent<RandomMove>();
                mover.halfworldSize = halfworldSize;
                mover.isNeedRotate = type == EShape2D.OBB;
            }

            proxy.IsStatic = isStatic;
            proxy.LayerType = layerType;
            return proxy;
        }

        public int showTreeId = 0;

        private void Update(){
            collisionSystem.ShowTreeId = showTreeId;
            collisionSystem.DoUpdate(Time.deltaTime.ToLFloat());
            ////class version 1.41ms
            //Profiler.BeginSample("CheckCollision");
            //CheckCollision();
            //Profiler.EndSample();
            ////0.32~0.42ms
            //Profiler.BeginSample("UpdateObj");
            //CheckUpdate();
            //Profiler.EndSample();
        }


        void OnDrawGizmos(){
            collisionSystem?.DrawGizmos();
        }
    }
}