using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChaiAurCharcha.UI.Net
{
    internal class Server
    {
        private TcpClient _tcpClient;

        private const string LOCAL_SERVER_IP = "127.0.0.1";
        private const int PORT = 7891;

        public Server()
        {
            this._tcpClient = new TcpClient();
        }

        public void ConnectToServer(string ip, int port)
        {
            this._tcpClient.Connect(ip, port);
        }

        public void Connect(string ipAddress, int port)
        {
            try
            {
                if (!_tcpClient.Connected)
                {
                    Console.WriteLine("Attempting to connect to server at {0}:{1}", LOCAL_SERVER_IP, PORT);
                    this._tcpClient.ConnectAsync(host: LOCAL_SERVER_IP, port: PORT);
                }

                Console.WriteLine("Connected to server at {0}:{1}", ipAddress, port);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }
        }
    }
}
