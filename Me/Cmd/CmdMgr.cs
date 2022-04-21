using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using static ServerCore.Define;

namespace Me
{
    public class CmdMgr
    {
        static CmdMgr _inst = new CmdMgr();
        public static CmdMgr Inst { get { return _inst; } }
        Dictionary<string, Func<Cmd>> _cmdDic;
        ConcurrentQueue<Cmd> _cmdQueue;
        CmdMgr()
        {
            _cmdDic = new Dictionary<string, Func<Cmd>>();
            _cmdDic.Add("login", () => new Cmd_Login());
            _cmdQueue = new ConcurrentQueue<Cmd>();
        }

        public void Run()
        {
            while (true)
            {
                if (!_cmdQueue.TryPeek(out Cmd cmd))
                    GetNewCmd();
                cmd?.Perform();
            }
        }
        public void GetNewCmd()
        {
            Console.Write("> ");
            var inst = Console.ReadLine().ToLower();
            _cmdDic.TryGetValue(inst, out Func<Cmd> factory);
            if(factory == null)
            {
                Console.WriteLine(inst + "??\n");
                return;
            }
            _cmdQueue.Enqueue(factory.Invoke());
        }
        public bool UpdateState(Cmd_State state)
        {
            var result = _cmdQueue.TryPeek(out Cmd cmd);
            if (result)
                cmd.UpdateState(state);
            return result;         
        }
        public void Enqueue(Cmd cmd)
        {
            _cmdQueue.Enqueue(cmd);
        }
        public Cmd Dequeue()
        {
            if (_cmdQueue.TryDequeue(out Cmd cmd))
                return cmd;
            return null;
        }
    }
}
