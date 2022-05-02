
using ServerCore;
using ServerCore.Log;
using System;
using System.Diagnostics;
using System.Net;

namespace Me
{
    class Program
    {
        static void Main(string[] args)
        {
            LogMgr.ChooseSaveDir("Client");
            var ts = LogMgr.AddNewSource("Client", SourceLevels.Information);
            LogMgr.AddNewConsoleListener("Client", "console", true, SourceLevels.Warning, TraceOptions.DateTime);
            LogMgr.AddNewTextWriterListener("Client", "text", "clientLog.txt", SourceLevels.Information ,TraceOptions.DateTime);
            ts.TraceEvent(TraceEventType.Information, 0, "Start client");

            string host = Dns.GetHostName();
            var ip = Dns.GetHostEntry(host);
            var ipAddress = ip.AddressList[0];
            var endPoint = new IPEndPoint(ipAddress, 1234);
            Connector.Inst.Init(endPoint, () => SessionMgr.Inst.GenerateSession<ServerSession>());
            Connector.Inst.Connect(1);

            JobMgr.Inst.CreateJobQueue("Send", 33, true);
            JobMgr.Inst.Push("Send", SessionMgr.Inst.Flush_Send);

            CmdMgr.Inst.Run();

            
        }
    }
}
