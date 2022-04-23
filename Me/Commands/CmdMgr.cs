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
            _cmdDic.Add("help", () => new Cmd_Help());
            _cmdDic.Add("login", () => new Cmd_Login());
            _cmdDic.Add("logout", () => new Cmd_Logout());
            _cmdDic.Add("update_lobby", () => new Cmd_UpdateLobby());
            _cmdDic.Add("update_room", () => new Cmd_UpdateRoom());
            _cmdDic.Add("enter_room", () => new Cmd_EnterRoom());
            _cmdDic.Add("exit", () => new Cmd_ExitRoom());
            _cmdDic.Add("chat", () => new Cmd_Chat());
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
            var inst = Console.ReadLine().ToLower().Split(' ');
            if (inst.Length == 0)
                return;
            string key = inst[0];
            _cmdDic.TryGetValue(key, out Func<Cmd> factory);
            if(factory == null)
            {
                Console.WriteLine(key + "??\n");
                return;
            }
            var cmd = factory.Invoke();
            if(inst.Length > 1)
            {
                if (!cmd.PutOption(inst[1..]))
                {
                    Console.WriteLine("Invalid Option");
                    return;
                }
            }
            _cmdQueue.Enqueue(cmd);
        }
        public bool UpdateState(Cmd_State state, string message = null)
        {
            var result = _cmdQueue.TryPeek(out Cmd cmd);
            if (result)
                cmd.UpdateState(state, message);
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
