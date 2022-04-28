using ServerCore;
using ServerCore.Log;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Server
{
    class Program
    {

        static void Main(string[] args)
        {
            LogMgr.ChooseSaveDir("Server");
            var ts = LogMgr.AddNewSource("Server",SourceLevels.Information);
            LogMgr.AddNewConsoleListener("Server", "console", false, SourceLevels.Warning , TraceOptions.DateTime);
            LogMgr.AddNewTextWriterListener("Server", "text","serverLog.txt", SourceLevels.Information, TraceOptions.DateTime);
            
            ts.TraceEvent(TraceEventType.Information, 0, "Start server");

            Listener _listener = new Listener();
            string host = Dns.GetHostName();
            var ip = Dns.GetHostEntry(host);
            var ipAddress = ip.AddressList[0];
            var endPoint = new IPEndPoint(ipAddress, 1234);
            _listener.Init(endPoint, ()=>SessionMgr.Inst.GenerateSession<ClientSession>());
            _listener.Open();
            
            JobMgr.Inst.CreateJobQueue("Send", 250, true);
            JobMgr.Inst.Push("Send", SessionMgr.Inst.Flush_Send);
            while (true)
            {

            }
        }
    }
}
