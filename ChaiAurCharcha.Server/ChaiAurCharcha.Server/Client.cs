using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChaiAurCharcha.Server
{
    internal class Client
    {
        public string UserName { get; set; }

        public Guid UID { get; set; }

        public TcpClient ClientSocket { get; set; }

        public Client(TcpClient clientSocket)
        {
            ClientSocket = clientSocket;
            UID = Guid.NewGuid();

            Console.WriteLine($"{DateTime.Now}: Client is connected with the username {UserName}");
        }
    }
}
