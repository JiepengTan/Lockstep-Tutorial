using System;
using System.Collections.Generic;
namespace Lockstep.Util {
    public interface IRecyclable {
        void OnReuse();
        void OnRecycle();
    }

    public class BaseRecyclable : IRecyclable {
        public virtual void OnReuse(){ }
        public virtual void OnRecycle(){ }
        public override string ToString(){
            return JsonUtil.ToJson(this);
        }
    }

    public class Pool {
        public static void Return<T>(T val) where T : IRecyclable, new(){
            Pool<T>.Return(val);
        }

        public static T Get<T>() where T : IRecyclable, new(){
            return Pool<T>.Get();
        }
    }

    public class Pool<T> where T : IRecyclable, new() {
        private static Stack<T> pool = new Stack<T>();


        public static T Get(){
            if (pool.Count == 0) {
                return new T();
            }
            else {
                return pool.Pop();
            }
        }

        public static void Return(T val){
            if (val == null) return;
            val.OnRecycle();
            pool.Push(val);
        }
    }
}