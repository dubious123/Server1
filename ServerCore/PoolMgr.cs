using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ServerCore
{
    public class PoolMgr
    {
        static PoolMgr _inst = new PoolMgr();
        public static PoolMgr Inst { get { return _inst; } }
        ConcurrentDictionary<string, Pool> _poolDict = new ConcurrentDictionary<string, Pool>();
        public Pool CreateNewPool<T>(string name, Func<T> factory,int count = 10) where T : class
        {
            var pool = new Pool(factory, count);
            if (_poolDict.TryAdd(name, pool) == false)
                throw new Exception($"Failed to add {name}");
            return pool;
        }
        public T Pull<T>(string name) where T : class
        {
            if (_poolDict.TryGetValue(name, out var pool) == false)
                throw new Exception($"Failed to get value, Key : {name}");
            var obj = pool.Pop();
            if (obj is T == false)
                throw new Exception($"Failed to cast item to {typeof(T)}");

            return obj as T;
        }
        public void Push<T>(string name, T obj) where T : class
        {
            if (_poolDict.TryGetValue(name, out var pool))
                throw new Exception($"Failed to get value, Key : {name}");
            pool.Push(obj);
        }

    }
}
