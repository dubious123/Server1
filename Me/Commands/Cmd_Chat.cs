using ServerCore;
using ServerCore.Packets.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Me
{
    public class Cmd_Chat : Cmd
    {
        string _chat;
        public override bool PutOption(params string[] options)
        {
            _chat = "";
            foreach (var s in options)
            {
                _chat += s;
                _chat += " ";
            }
            return true;
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
            if(_chat == "")
            {
                Console.WriteLine("invalid option : no chat");
                CmdMgr.Inst.Dequeue();
                return;
            }
            Console.WriteLine();
            Console.Write("sending");
            Chatting chat = new Chatting(ContentMgr.Inst.User, _chat);
            C_Chat c_packet = new C_Chat(chat);
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
