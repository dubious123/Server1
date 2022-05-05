using ServerCore.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Listener
    {
        Socket _socket;
        IPEndPoint _endPoint;
        Func<Session> _sessionFactory;
        TraceSource _ts;
        int _textListenerIndex;
        public void Init(IPEndPoint endPoint, Func<Session> func)
        {
            _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _endPoint = endPoint;
            _sessionFactory += func;
            _ts = LogMgr.AddNewSource("Listener", SourceLevels.Information);
            _textListenerIndex = 
                LogMgr.AddNewTextWriterListener("Listener", "l_Listener", "listenLog.txt", SourceLevels.Information, TraceOptions.DateTime, TraceOptions.ThreadId);
            LogMgr.AddNewConsoleListener("Listener", "c_Listener", false,  SourceLevels.Warning, TraceOptions.DateTime, TraceOptions.ThreadId);
        }
        public void Open()
        {
            _socket.Bind(_endPoint);
            _socket.Listen(2000);
            for(int i = 0; i < 100; i++)
            {               
                var args = new SocketAsyncEventArgs();
                args.Completed += OnAcceptCompleted;
                RegisterAccept(args);

            }

        }
        void RegisterAccept(SocketAsyncEventArgs args)
        {
            _ts.TraceInfo($"Throw new receive rod from thread [{Thread.CurrentThread.ManagedThreadId}]");
            args.AcceptSocket = null;
            bool pending = _socket.AcceptAsync(args);
            if (pending == false)
                OnAcceptCompleted(this, args);        
        }
        
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                _ts.TraceInfo($"Accept success from thread : [{Thread.CurrentThread.ManagedThreadId}]");
                var session = _sessionFactory.Invoke();
                session.Init(args.AcceptSocket);
                session.OnConnected();
            }
            else
            {
                _ts.TraceEvent(TraceEventType.Warning, 3, $"Accept socket failed from thread : [{Thread.CurrentThread.ManagedThreadId}]\n" +
                    $"  error : {args.SocketError}");
            }
            RegisterAccept(args);
        }
    }
}
