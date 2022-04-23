using ServerCore;
using ServerCore.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Me
{
    public class Cmd_UpdateRoom : Cmd
    {
        public override bool CheckUserCondition()
        {
            return ContentMgr.Inst.User.UserState == Define.User_State.room;
        }
        public override bool PutOption(params string[] options)
        {
            return false;
        }

        public override void Start()
        {
            if(CheckUserCondition() == false)
            {
                Console.WriteLine("You are not in room");
                CmdMgr.Inst.Dequeue();
                return;
            }
            C_UpdateRoom c_packet = new C_UpdateRoom(ContentMgr.Inst.User);
            ContentMgr.Inst.GetSession().RegisterSend(c_packet);
            CmdMgr.Inst.UpdateState(Define.Cmd_State.wait);
           
        }
        public override void Done()
        {
            CmdMgr.Inst.Dequeue();
        }

        public override void Error()
        {
            throw new NotImplementedException();
        }


    }
}
