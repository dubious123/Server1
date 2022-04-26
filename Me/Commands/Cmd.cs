using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ServerCore.Define;
using ServerCore;
using System.Diagnostics;
using ServerCore.Log;

namespace Me
{
    public abstract class Cmd
    {
        protected TraceSource _ts;
        public Cmd(Cmd_State state = Cmd_State.start, int waitLimit = 10)
        {
            _state = state;
            _waitLimit = waitLimit;
            _ts = LogMgr.GetTraceSource("Client");
        }
        public Cmd(Cmd_State state)
        {
            _state = state;
            _ts = LogMgr.GetTraceSource("Client");
        }
        Cmd_State _state;
        int _waitLimit;
        int _waitCount;
        protected string _errorMessage;
        
        public virtual void Perform()
        {
            if (SessionMgr.Inst.Find(1).GetSocketState() == false)
            {
                Console.WriteLine("you are not connected to the server");
                CmdMgr.Inst.Clear();
                return;
            }
            switch (_state)
            {
                case Cmd_State.start:
                    Start();
                    break;
                case Cmd_State.wait:
                    Waiting();
                    break;
                case Cmd_State.done:
                    Done();
                    break;
                case Cmd_State.error:
                    Error();
                    break;
                default:
                    throw new Exception();
            }
        }
        public virtual void Waiting()
        {
            Console.Write(".");
            Thread.Sleep(1000);
            if (_waitCount++ < _waitLimit)
                return;
            _waitCount = 0;
            UpdateState(Cmd_State.error, "No response from server");
        }
        readonly static object _stateLock = new object();
        public virtual void UpdateState(Cmd_State state, string message = null)
        {
            lock (_stateLock)
            {
                _state = state;
                if (message != null)
                    _errorMessage = message;
            }         
        }
        public abstract bool PutOption(params string[] options);
        public abstract bool CheckUserCondition();
        public abstract void Start();
        public abstract void Done();
        public abstract void Error();
    }
}
