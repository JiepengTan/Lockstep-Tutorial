using System;
using Lockstep.Math;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Lockstep.Collision2D {
#if UNITY_5_3_OR_NEWER
    public class ColliderDataMono : UnityEngine.MonoBehaviour {
        public ColliderData colliderData;
    }
#endif
    [Serializable]
    public partial class ColliderData :IComponent{
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