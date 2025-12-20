using System.Net;
using System.Net.Sockets;

namespace ChaiAurCharcha.Server
{
    internal class Program
    {
        private static TcpListener _tcpListener;

        internal static void Main(string[] args)
        {
            _tcpListener = new(IPAddress.Parse("127.0.0.1"), 7891);

            _tcpListener.Start();

            var tcpClient = _tcpListener.AcceptTcpClient();
            Console.WriteLine("Client is connected now!");
        }
    }
}
