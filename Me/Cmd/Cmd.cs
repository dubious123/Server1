using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ServerCore.Define;

namespace Me
{
    public abstract class Cmd
    {
        
        public Cmd(Cmd_State state = Cmd_State.start, int waitLimit = 10)
        {
            _state = state;
            _waitLimit = waitLimit;
        }
        Cmd_State _state;
        int _waitLimit;
        int _waitCount;
        protected string _errorMessage;
        public void Perform()
        {
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
        public abstract void Start();
        public abstract void Done();
        public abstract void Error();
    }
}
