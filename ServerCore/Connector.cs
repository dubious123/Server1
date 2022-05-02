using ServerCore.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Connector
    {
        static Connector _inst = new Connector();
        public static Connector Inst { get { return _inst; } }
        IPEndPoint _endPoint;
        Func<Session> _sessionFactory;
        TraceSource _ts;
        public void Init(IPEndPoint endPoint, Func<Session> func)
        {
            _endPoint = endPoint;
            _sessionFactory += func;
            _ts = LogMgr.AddNewSource("Connector", SourceLevels.Information);
            LogMgr.AddNewTextWriterListener("Connector", "l_Connector", "connectLog.txt", SourceLevels.Information, TraceOptions.DateTime);
            LogMgr.AddNewConsoleListener("Connector","c_Connector" ,false, SourceLevels.Warning, TraceOptions.DateTime);          
        }
        public void Connect(int count, object session = null)
        {
            for(int i = 1; i<= count; i++)
            {
                _ts.TraceInfo($"[connector] session[{i}] connecting to server ");
                var args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = _endPoint;
                var socket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                bool pending = socket.ConnectAsync(args);
                if (pending == false)
                    OnConnectCompleted(session, args);
            }
        }
        public void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                
                if (sender is not Session session)
                    session = _sessionFactory.Invoke();
                _ts.TraceInfo($"[connector] session [{session.SessionID}] connect success");
                session.Init(args.ConnectSocket);
                session.OnConnected();
            }
            else
            {
                OnConnectFailed(args);
            }
        }
        public void OnConnectFailed(SocketAsyncEventArgs arg)
        { 
            _ts.TraceEvent(TraceEventType.Warning, 1, $"[connector] connect failed {arg.SocketError}");
            Connect(1);
        }
        
    }
}
