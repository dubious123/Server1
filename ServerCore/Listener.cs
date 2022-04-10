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
        public void Init(IPEndPoint endPoint)
        {
            _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _endPoint = endPoint;
        }
        public void Open()
        {
            _socket.Bind(_endPoint);
            _socket.Listen(100);
            var args = new SocketAsyncEventArgs();
            args.Completed += OnRecvCompleted;
            RegisterRecv(args);
        }
        void RegisterRecv(SocketAsyncEventArgs args)
        {
            if (!_socket.ReceiveAsync(args))
                OnRecvCompleted(null, args);
        }
        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Console.WriteLine("Recieved Packet");
            }
            else
            {
                Console.WriteLine($"Socket Recieve Failed : {args.SocketError}");
            }
            RegisterRecv(args);
        }
    }
}
