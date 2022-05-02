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
        Thread _thread;
        ConcurrentQueue<Action> _jobQueue = new ConcurrentQueue<Action>();
        public JobQueue(string name, int waitTick)
        {
            _name = name;
            _now = 0;
            _waitTick = waitTick;
        }
        public void Start()
        {
            _thread = new Thread(Loop);
            _thread.Name = _name;
            _thread.Start();
        }
        public void Push(Action action, int num = 1)
        {
            for(int i = 0; i<num; i++)
            {
                lock (_lock)
                {
                    _jobQueue.Enqueue(action);
                }
            }
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
