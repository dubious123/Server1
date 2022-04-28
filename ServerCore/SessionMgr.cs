using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Concurrent;
using ServerCore.Log;
using System.Diagnostics;

namespace ServerCore
{
    public class SessionMgr
    {
        static SessionMgr _inst = new SessionMgr();
        public static SessionMgr Inst { get { return _inst; } }
        uint _sessionCount = 0;
        ConcurrentDictionary<uint, Session> _sessionDict;
        TraceSource _ts;
        SessionMgr()
        {
            _sessionDict = new ConcurrentDictionary<uint, Session>();
            _ts = LogMgr.AddNewSource("Session", SourceLevels.Information);
            LogMgr.AddNewTextWriterListener("Session", "sessionListener", "sessionLog.txt", SourceLevels.Information, TraceOptions.DateTime, TraceOptions.ThreadId);
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
            _ts.TraceInfo($"[SessionMgr] session {inst.SessionID} generated");

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
            _ts.TraceInfo($"[SessionMgr] removing session {sessionID}");
            if (_sessionDict.TryRemove(sessionID, out Session session) == false)
                _ts.TraceEvent(TraceEventType.Error, 0, "[SessionMgr] Removing session failed : session Id is not in the dict");
        }
        public Session Find(uint id)
        {
            _ts.TraceInfo($"[SessionMgr] Finding session {id}");
            if(_sessionDict.TryGetValue(id, out Session value) == false)
                _ts.TraceEvent(TraceEventType.Error, 1, "[SessionMgr] Finding session failed : session Id is not in the dict");
            return value;
        }
    }
}
