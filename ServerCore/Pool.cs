using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace ServerCore
{
    public class Pool
    {
        dynamic _func;
        ConcurrentStack<object> _stack;
        int _maxCount;
        public Pool(dynamic func, int count) 
        {
            _func = func;
            _stack = new ConcurrentStack<object>();
            _maxCount = count;
            for (int i = 0; i < count; i++)
                _stack.Push(_func.Invoke());
        }
        public object Pop()
        {           
            if(_stack.TryPop(out var item) == false)
            {
                var j = _maxCount / 2;
                for(int i = 0; i < j; i++)
                    _stack.Push(_func.Invoke());
                Interlocked.Add(ref _maxCount, j);
                return Pop();               
            }
            return item;
        }
        public void Push<T>(T item)
        {
            _stack.Push(item);
        }
    }
}
