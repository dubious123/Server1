using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{

    public class JobQueue
    {
        string _name;
        int _waitTick;
        long _now;
        object _lock = new object();
        ManualResetEvent _resetEvent = new ManualResetEvent(true);
        Thread[] _threads;
        ConcurrentQueue<Action> _jobQueue = new ConcurrentQueue<Action>();
        public JobQueue(string name, int waitTick, int threadNum)
        {
            _name = name;
            _now = 0;
            _waitTick = waitTick;
            _threads = new Thread[threadNum];
            for (int i = 0; i < threadNum; i++)
            {
                _threads[i] = new Thread(Loop);
                _threads[i].Name = $"{_name}[{i}]";
            }


        }
        public void Start()
        {
            foreach (var t in _threads)
                t.Start();
        }
        public void Push(Action action)
        {
            _jobQueue.Enqueue(action);
        }
        public void Stop()
        {
            _resetEvent.Reset();
        }
        public void Resume()
        {
            _resetEvent.Set();
        }
        void Loop()
        {
            while (true)
            {
                if (_waitTick < Environment.TickCount64 - _now)
                {
                    for (int j = 0; j < _jobQueue.Count; j++)
                    {
                        if (_jobQueue.TryDequeue(out var action))
                            action.Invoke();                       
                    }
                    _now = Environment.TickCount64;
                }
            }           
        }
    }
}
