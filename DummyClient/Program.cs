using System.Net.Sockets;
using System;
using System.Net;
using System.Threading;

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
            byte[] fromServer = new byte[1];
            Console.WriteLine(fromServer[0]);
            for (; true; )
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(ipAddress, 1234);
                socket.Send(new byte[] { 1 });
                Thread.Sleep(250);
                socket.Receive(fromServer);
                Console.WriteLine(fromServer[0]);
            }
        }
    }
}
