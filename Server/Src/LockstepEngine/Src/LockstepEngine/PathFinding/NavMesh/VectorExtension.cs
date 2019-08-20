using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Lockstep.Math;

namespace Lockstep.PathFinding {
    public static class VectorExtension {
        public static LVector3 set(this LVector3 vec, LFloat x, LFloat y, LFloat z){
            vec.x = x;
            vec.y = y;
            vec.z = z;
            return vec;
        }

        public static LVector3 set(this LVector3 vec, LVector3 val){
            vec = val;
            return vec;
        }

        public static LVector3 mulAdd(this LVector3 _this, LVector3 vec, LFloat scalar){
            _this.x += vec.x * scalar;
            _this.y += vec.y * scalar;
            _this.z += vec.z * scalar;
            return _this;
        }

        public static LVector3 Add(this LVector3 vec, LVector3 val){
            return vec + val;
        }

        public static LVector3 sub(this LVector3 vec, LVector3 val){
            return vec - val;
        }

        public static LVector3 scl(this LVector3 vec, LFloat val){
            return vec * val;
        }
      
        public static LFloat dot(this LVector3 vec, LVector3 val){
            return LVector3.Dot(vec, val);
        }
        public static LFloat dot(this LVector3 vec, LFloat x, LFloat y, LFloat z){
            return LVector3.Dot(vec, new LVector3(x, y, z));
        }


        public static LVector3 cross(this LVector3 vec, LVector3 vector){
            return new LVector3(vec.y * vector.z - vec.z * vector.y, vec.z * vector.x - vec.x * vector.z,
                vec.x * vector.y - vec.y * vector.x);
        }

        public static LVector3 cross(this LVector3 vec, LFloat x, LFloat y, LFloat z){
            return new LVector3(vec.y * z - vec.z * y, vec.z * x - vec.x * z, vec.x * y - vec.y * x);
        }

        public static LVector3 nor(this LVector3 vec){
            return vec.normalized;
        }

        public static LFloat len(this LVector3 vec){
            return vec.magnitude;
        }

        public static LFloat dst2(this LVector3 vec, LVector3 p){
            return dst2(vec.x, vec.z, p.x, p.z);
        }

        public static LFloat dst2(LFloat x1, LFloat z1, LFloat x2, LFloat z2){
            x1 -= x2;
            z1 -= z2;
            return (x1 * x1 + z1 * z1);
        }

        public static T get<T>(this List<T> lst, int idx){
            return lst[idx];
        }

        public static Tval get<Tkey, Tval>(this Dictionary<Tkey, Tval> lst, Tkey idx){
            return lst[idx];
        }
    }
}