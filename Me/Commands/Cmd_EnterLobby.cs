using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Me
{
    public class Cmd_EnterLobby : Cmd
    {
        public override bool PutOption(params string[] paramList)
        {
            return false;
        }
        public override void Start()
        {
            Console.WriteLine();
            if (CheckUserCondition() == false)
            {
                Console.WriteLine("You are not logged in");
                CmdMgr.Inst.Dequeue();
                return;
            }
            C_EnterLobby c_packet = new C_EnterLobby(ContentMgr.Inst.User);
            var bytes = PacketMgr.Inst.PacketToByte(c_packet);
            SessionMgr.Inst.Find(1).RegisterSend(bytes);
            Console.Write("Entering lobby");
            UpdateState(Define.Cmd_State.wait);
        }
        public override void Done()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Entered lobby");
            CmdMgr.Inst.Dequeue();
            CmdMgr.Inst.Enqueue(new Cmd_UpdateLobby());
        }

        public override void Error()
        {
            Console.WriteLine();
            Console.WriteLine("Enter lobby failed : " + _errorMessage);
            CmdMgr.Inst.Dequeue();
        }

        public override bool CheckUserCondition()
        {
            return ContentMgr.Inst.User.UserState == Define.User_State.login;
        }
    }
}
