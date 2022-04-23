using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Me
{
    public class Cmd_Help : Cmd
    {
        public override bool CheckUserCondition()
        {
            return true;
        }

        public override void Done()
        {
            throw new NotImplementedException();            
        }

        public override void Error()
        {
            throw new NotImplementedException();
        }

        public override bool PutOption(params string[] options)
        {
            return false;
        }

        public override void Start()
        {
            Console.WriteLine(
@"
    help 
    enter_lobby
    enter_room {room number}
    exit
    login
    logout
    update_lobby
    update_room
    chat {message}
");
            CmdMgr.Inst.Dequeue();
        }
    }
}
