using System;
using System.Collections;
using System.Collections.Generic;

namespace Lockstep.Util {
    public class Buffer<T> : IDisposable, IEnumerable<T>, IEnumerable where T : struct {
        private static Stack<Buffer<T>> _pool = new Stack<Buffer<T>>();
        private bool _inited;
        public T[] Items;
        public int Count;

        public T this[int index] {
            get {
                if (index >= 0 && index < this.Count)
                    return this.Items[index];
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public static Buffer<T> Alloc(int size){
            lock (Buffer<T>._pool) {
                while (Buffer<T>._pool.Count > 0) {
                    Buffer<T> buffer = Buffer<T>._pool.Pop();
                    if (buffer._inited) {
                        if (buffer.Items.Length < size)
                            buffer.Items = new T[size];
                        buffer._inited = false;
                        return buffer;
                    }
                }
            }

            return new Buffer<T>(size);
        }

        private Buffer(int size){
            this.Items = new T[size];
        }

        public void Add(T item){
            this.Items[this.Count++] = item;
        }

        public void Dispose(){
            if (this._inited)
                return;
            this.Count = 0;
            this._inited = true;
            lock (Buffer<T>._pool)
                Buffer<T>._pool.Push(this);
        }

        public IEnumerator<T> GetEnumerator(){
            if (this._inited)
                throw new Exception("Buffer has been pooled");
            for (int i = 0; i < this.Count; ++i) {
                if (this._inited)
                    throw new Exception("Buffer has been pooled");
                yield return this.Items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator(){
            return (IEnumerator) this.GetEnumerator();
        }
    }
}