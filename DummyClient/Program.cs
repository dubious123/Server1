using System.Net.Sockets;
using System;
using System.Net;
using System.Threading;
using ServerCore;
using ServerCore.Log;
using System.Diagnostics;

namespace DummyClient
{
    class Program
    {
        
        static void Main(string[] args)
        {
            LogMgr.ChooseSaveDir("DummyClient");
            var ts = LogMgr.AddNewSource("Dummy", SourceLevels.Information);
            LogMgr.AddNewTextWriterListener("Dummy", "listener", "DummyClient.txt", SourceLevels.Information, TraceOptions.DateTime);
            LogMgr.AddNewConsoleListener("Dummy", "c_listener", false, SourceLevels.Information, TraceOptions.DateTime);
            string host = Dns.GetHostName();
            var ip = Dns.GetHostEntry(host);
            var ipAddress = ip.AddressList[0];
            var endPoint = new IPEndPoint(ipAddress, 1234);
            Connector _connector = new Connector();
            _connector.Init(endPoint, () => SessionMgr.Inst.GenerateSession<D_ServerSession>());
            _connector.Connect(1000);

            JobMgr.Inst.CreateJobQueue("Send", 33, true);
            JobMgr.Inst.CreateJobQueue("Dummy", 0, false);
            JobMgr.Inst.Push("Send", SessionMgr.Inst.Flush_Send);
            JobMgr.Inst.Push("Dummy", DummyMgr.Inst.ControlDummies);
            Console.WriteLine("press any key to create dummies");
            Console.ReadLine();
            DummyMgr.Inst.Init(1000);
            Console.WriteLine("press any key to start dummies");
            Console.ReadLine();
            JobMgr.Inst.StartQueue("Dummy");
            while (true)
            {
                Thread.Sleep(System.Threading.Timeout.Infinite);
            }
        }
    }
}
