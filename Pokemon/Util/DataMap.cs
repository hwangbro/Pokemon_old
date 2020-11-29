using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Pokemon {

    public class DataMap<T> : List<T> {

        public Dictionary<Type, Func<T, object, bool>> Callbacks;

        public DataMap() : base() {
            Callbacks = new Dictionary<Type, Func<T, object, bool>>();
        }

        public T this[object o] {
            get {
                Type t = o.GetType();
                Debug.Assert(Callbacks.ContainsKey(t), "Unknown type: {0}", t);
                Func<T, object, bool> callback = Callbacks[t];
                return this.FirstOrDefault(value => callback(value, o));
            }
        }
    }

    public class DataArray<T> : IEnumerable<T> {

        public T[,] Array;
        public int Width;
        public int Height;
        public Dictionary<Type, Func<T, object, bool>> Callbacks;

        public DataArray(int width, int height) : this(width, height, new Dictionary<Type, Func<T, object, bool>>()) {}
        public DataArray(int width, int height, Dictionary<Type, Func<T, object, bool>> callbacks) {
            (Width, Height, Callbacks) = (width, height, callbacks);
            Array = new T[width, height];
        }

        public T this[byte x, byte y] {
            get { return Array[x, y]; }
            set { Array[x, y] = value; }
        }

        public T this[int x, int y] {
            get { return Array[x, y]; }
            set { Array[x, y] = value; }
        }

        public T this[uint x, uint y] {
            get { return Array[x, y]; }
            set { Array[x, y] = value; }
        }

        public T this[object o] {
            get {
                Type t = o.GetType();
                Debug.Assert(Callbacks.ContainsKey(t), "Unknown type: {0}", t);
                Func<T, object, bool> callback = Callbacks[t];
                return this.FirstOrDefault(value => callback(value, o));
            }
        }

        public IEnumerator<T> GetEnumerator() {
            foreach(var item in Array) {
                if(item != null) yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
