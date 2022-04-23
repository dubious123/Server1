using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore.Packets;
using ServerCore;

namespace Me
{
    public class Cmd_UpdateLobby : Cmd
    {
        public Cmd_UpdateLobby() : base() { }
        public Cmd_UpdateLobby(Define.Cmd_State state) : base(state) { }
        public override bool PutOption(params string[] options)
        {
            return false;
        }
        public override void Start()
        {
            Console.WriteLine( );
            if (CheckUserCondition() == false)
            {
                Console.WriteLine("You are not in lobby");
                CmdMgr.Inst.Dequeue();
                return;
            }
            Console.Write("Loading lobby");
            C_UpdateLobby c_packet = new C_UpdateLobby(ContentMgr.Inst.User);
            SessionMgr.Inst.Find(1).RegisterSend(c_packet);
            UpdateState(Define.Cmd_State.wait);
        }
        public override void Done()
        {
            ContentMgr.Inst.ShowLobby();
            CmdMgr.Inst.Dequeue();
        }

        public override void Error()
        {
            Console.WriteLine();
            Console.WriteLine($"Lobby Update Failed : {_errorMessage}");
            Console.WriteLine("Retry? Y/N");
            while (true)
            {
                string cmd = Console.ReadLine().ToLower();
                if (cmd == "y")
                {
                    UpdateState(Define.Cmd_State.start);
                    return;
                }
                else if (cmd == "n")
                {
                    CmdMgr.Inst.Dequeue();
                    return;
                }
                else
                {
                    Console.WriteLine($"{cmd} ??");
                }
            }

        }

        public override bool CheckUserCondition()
        {
            return ContentMgr.Inst.User.UserState == Define.User_State.lobby;
        }
    }
}
