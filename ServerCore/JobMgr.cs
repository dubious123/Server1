using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public class JobMgr
    {
        static JobMgr _inst = new JobMgr();
        public static JobMgr Inst { get { return _inst; } }
        Dictionary<string, JobQueue> _jobDict;
        object _lock = new object();
        JobMgr()
        {
            _jobDict = new Dictionary<string, JobQueue>();

        }
        public void CreateJobQueue(string name, int waitTick, bool startNow)
        {
            JobQueue queue = new JobQueue(name, waitTick);
            if (_jobDict.ContainsKey(name))
            {
                Console.WriteLine("JobQueue name already exists");
                return;
            }
            _jobDict.Add(name, queue);
            if (startNow)
                StartQueue(name);
        }
        public void Push(string name, Action action)
        {
            _jobDict[name]?.Push(action);
        }
        public void StartQueue(string name)
        {
            _jobDict[name].Start();
        }
        public void StopQueue(string name)
        {
            _jobDict[name].Stop();
        }
    }
}
