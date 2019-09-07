using System;
using Lockstep.Serialization;

namespace Lockstep.Serialization {
    public class NoToBytesAttribute : Attribute { }

    public class ToBytesAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IndexAttribute : Attribute {
        public int idx;

        public IndexAttribute(int idx){
            this.idx = idx;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TypeIdAttribute : Attribute {
        public int idx;

        public TypeIdAttribute(int idx){
            this.idx = idx;
        }
    }

    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ExtFormatAttribute : Attribute { }
    
    public static class NetMsgExtension {

        public static T Parse<T>(this Deserializer reader) where T : ISerializable,new (){
            var val = new T();
            val.Deserialize(reader);
            return val;
        }
    }
    
    
    public static class ArrayExtention {
        public static bool EqualsEx(this byte[] arra, byte[] arrb) {
            if ((arra == null) != (arrb == null)) return false;
            if (arra == null) {
                return true;
            }
            var count = arra.Length;
            if (count != arrb.Length) {
                return false;
            }
            for (int i = 0; i < count; i++) {
                if (arra[i] != arrb[i]) {
                    return false;
                }
            }

            return true;
        }

        public static bool EqualsEx<T>(this T[] arra, T[] arrb) where T : class{
            if ((arra == null) != (arrb == null)) return false;
            if (arra == null) {
                return true;
            }

            var count = arra.Length;
            if (count != arrb.Length) {
                return false;
            }

            for (int i = 0; i < count; i++) {
                var a = arra[i];
                var b = arrb[i];
                if ((a == null) != (b == null)) return false;
                if (a == null) {
                    continue;
                }

                if (!a.Equals(b)) {
                    return false;
                }
            }

            return true;
        }
    }
}