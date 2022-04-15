using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Connector
    {
        IPEndPoint _endPoint;
        Func<Session> _sessionFactory;
        public void Init(IPEndPoint endPoint, Func<Session> func)
        {
            _endPoint = endPoint;
            _sessionFactory += func;
        }
        public void Connect()
        {
            var args = new SocketAsyncEventArgs();
            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = _endPoint;
            var socket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            bool pending = socket.ConnectAsync(args);
            if (pending == false)
                OnConnectCompleted(this, args);
        }
        public void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                var session = _sessionFactory.Invoke();
                session.Init(args.ConnectSocket);
                session.OnConnected();
            }
            else
            {
                OnConnectFailed();
            }
        }
        public void OnConnectFailed()
        {
            Console.WriteLine("Packet connect failed");
        }
        
    }
}
