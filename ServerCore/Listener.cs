using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Listener
    {
        Socket _socket;
        IPEndPoint _endPoint;
        Func<Session> _sessionFactory;
        public void Init(IPEndPoint endPoint, Func<Session> func)
        {
            _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _endPoint = endPoint;
            _sessionFactory += func;
        }
        public void Open()
        {
            _socket.Bind(_endPoint);
            _socket.Listen(100);
            var args = new SocketAsyncEventArgs();
            args.Completed += OnAcceptCompleted;
            RegisterAccept(args);
        }
        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;
            bool pending = _socket.AcceptAsync(args);
            if (pending == false)
                OnAcceptCompleted(this, args);
        }
        
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                var session = _sessionFactory.Invoke();
                session.Init(args.AcceptSocket);
                session.RegisterReceive();
            }
            else
            {
                Console.WriteLine($"Socket Accept Failed : {args.SocketError}");
            }
            RegisterAccept(args);
        }
    }
}
