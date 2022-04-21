using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Concurrent;

namespace ServerCore
{
    public class SessionMgr
    {
        static SessionMgr _inst = new SessionMgr();
        public static SessionMgr Inst { get { return _inst; } }
        uint _sessionCount = 0;
        ConcurrentDictionary<uint, Session> _sessionDict;
        SessionMgr()
        {
            _sessionDict = new ConcurrentDictionary<uint, Session>();
        }
        public uint GetSessionId()
        {
            return Interlocked.Increment(ref _sessionCount);
        }
        public T GenerateSession<T>() where T : Session, new()
        {
            var inst = new T();
            inst.SessionID = GetSessionId();
            
            var addResult = _sessionDict.TryAdd(inst.SessionID, inst);
            if (!addResult)
                throw new Exception();
            return inst;
        }
        public void Flush_Send()
        {
            foreach (var session in _sessionDict.Values)
            {
                if (session.SendRegistered)
                    session.Send();
            }
            JobMgr.Inst.Push("Send", Flush_Send);
        }
        public void Remove(uint sessionID)
        {
            _sessionDict.TryRemove(sessionID, out Session session);

        }
        public Session Find(uint id)
        {
            _sessionDict.TryGetValue(id,out Session value);
            return value;
        }
    }
}
