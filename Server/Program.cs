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
            string host = Dns.GetHostName();
            var ip = Dns.GetHostEntry(host);
            var ipAddress = ip.AddressList[0];
            var endPoint = new IPEndPoint(ipAddress, 1234);
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endPoint);
            socket.Listen();
            byte[] recieveContent = new byte[1];
            while (true)
            {
                var s = socket.Accept();
                if (s != null)
                    s.Receive(recieveContent);

                Console.WriteLine(recieveContent[0]);

                s.Send(new byte[1] { 1 });
                Thread.Sleep(100);
            }
        }
    }
}
