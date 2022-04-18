using System.Net.Sockets;
using System;
using System.Net;
using System.Threading;
using ServerCore;

namespace DummyClient
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
            _connector.Init(endPoint, () => new ServerSession());
            _connector.Connect(500);

            JobMgr.Inst.CreateJobQueue("Send", 0, true);
            JobMgr.Inst.CreateJobQueue("Json", 0, true);
            Console.ReadLine();
        }
    }
}
