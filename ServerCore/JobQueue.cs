using System;
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
        int _now;
        object _lock = new object();
        ManualResetEvent _resetEvent = new ManualResetEvent(true);
        Thread _thread;
        Queue<Action> _jobQueue = new Queue<Action>();
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
                //_resetEvent.WaitOne();
                if (_waitTick < Environment.TickCount - _now)
                {
                    lock (_lock)
                    {
                        var i = _jobQueue.Count;
                        for (int j = 0; j < i; j++) 
                        {
                            _jobQueue.Dequeue().Invoke();
                        }                  
                    }
                    _now = Environment.TickCount;
                }
            }           
        }
    }
}
