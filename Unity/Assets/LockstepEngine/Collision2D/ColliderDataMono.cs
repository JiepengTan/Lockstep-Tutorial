using System;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Collision2D {
#if UNITY_5_3_OR_NEWER
    public class ColliderDataMono : UnityEngine.MonoBehaviour {
        public ColliderData colliderData;
    }
#endif
    [Serializable]
    public class ColliderData {
#if UNITY_5_3_OR_NEWER
        [Header("Offset")]
#endif
        public LFloat y;
        public LVector2 pos;
#if UNITY_5_3_OR_NEWER
        [Header("Collider data")]
#endif
        public LFloat high;
        public LFloat radius;
        public LVector2 size;
        public LVector2 up;
        public LFloat deg;
        
    }
}