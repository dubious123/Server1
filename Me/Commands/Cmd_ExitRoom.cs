using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
using ServerCore.Packets.Client;

namespace Me
{
    public class Cmd_ExitRoom : Cmd
    {
        public override bool PutOption(params string[] options)
        {
            return false;
        }
        public override bool CheckUserCondition()
        {
            return ContentMgr.Inst.User.UserState == Define.User_State.room;
        }
        public override void Start()
        {
            if(CheckUserCondition() == false)
            {
                Console.WriteLine("you are not in room");
                CmdMgr.Inst.Dequeue();
                return;
            }
            C_ExitRoom c_packet = new C_ExitRoom(ContentMgr.Inst.User);
            ContentMgr.Inst.GetSession().RegisterSend(c_packet);
            Console.Write("Exit to lobby");
            CmdMgr.Inst.UpdateState(Define.Cmd_State.wait);

        }
        public override void Done()
        {
            CmdMgr.Inst.Dequeue();
            CmdMgr.Inst.Enqueue(new Cmd_UpdateLobby(Define.Cmd_State.done));
        }

        public override void Error()
        {
            throw new NotImplementedException();
        }




    }
}
