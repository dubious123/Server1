
using ServerCore;
using System;
using System.Net;

namespace Me
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            var ip = Dns.GetHostEntry(host);
            var ipAddress = ip.AddressList[0];
            var endPoint = new IPEndPoint(ipAddress, 1234);
            Connector _connector = new Connector();
            _connector.Init(endPoint, () => SessionMgr.Inst.GenerateSession<ServerSession>());
            _connector.Connect(1);

            JobMgr.Inst.CreateJobQueue("Send", 250, true);
            JobMgr.Inst.CreateJobQueue("Json", 0, true);
            JobMgr.Inst.Push("Send", SessionMgr.Inst.Flush_Send);

            CmdMgr.Inst.Run();

            
        }
    }
}
