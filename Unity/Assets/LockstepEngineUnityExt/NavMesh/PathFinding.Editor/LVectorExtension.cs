using Lockstep.Math;
using UnityEngine;


public static class LVectorExtension {
    public static Vector3[] ToVecArray(this LVector3[] lVecs){
        var vecs = new Vector3[lVecs.Length];
        for (int i = 0; i < lVecs.Length; i++) {
            vecs[i] = lVecs[i].ToVector3();
        }
        return vecs;
    }
    public static LVector3[] ToLVecArray(this Vector3[] lVecs){
        var vecs = new LVector3[lVecs.Length];
        for (int i = 0; i < lVecs.Length; i++) {
            vecs[i] = lVecs[i].ToLVector3();
        }
        return vecs;
    }
}
