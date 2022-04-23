using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
using ServerCore.Packets.Client;

namespace Me
{
    public class Cmd_Logout : Cmd
    {
        public override bool CheckUserCondition()
        {
            return ContentMgr.Inst.User.UserState != ServerCore.Define.User_State.logout;
        }
        public override bool PutOption(params string[] options)
        {
            return false;
        }

        public override void Start()
        {
            if(CheckUserCondition() == false)
            {
                Console.WriteLine();
                Console.WriteLine("You are not logged in");
                CmdMgr.Inst.Dequeue();
                return;
            }
            var c_packet = new C_Logout(ContentMgr.Inst.User);
            Console.WriteLine();
            Console.Write("Logging out");
            CmdMgr.Inst.UpdateState(Define.Cmd_State.wait);
            ContentMgr.Inst.GetSession().RegisterSend(c_packet);
            
        }
        public override void Done()
        {
            Console.WriteLine();
            Console.WriteLine("Logout completed");
            ContentMgr.Inst.Clear();
            CmdMgr.Inst.Dequeue();
        }

        public override void Error()
        {
            throw new Exception();
        }


    }
}
