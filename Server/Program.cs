using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Server
{
    class Program
    {

        static void Main(string[] args)
        {
            Listener _listener = new Listener();
            string host = Dns.GetHostName();
            var ip = Dns.GetHostEntry(host);
            var ipAddress = ip.AddressList[0];
            var endPoint = new IPEndPoint(ipAddress, 1234);
            _listener.Init(endPoint, ()=>new ClientSession());
            _listener.Open();
            JobMgr.Inst.CreateJobQueue("Send", 250, true);
            JobMgr.Inst.CreateJobQueue("Json", 0, true);
            while (true)
            {

            }
        }
    }
}
