using System.Net.Sockets;

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

        public void ConnectToServer()
        {
            Task.Run(() => ConnectServerAsync(ipAddress: LOCAL_SERVER_IP, port: PORT));
        }

        private async Task ConnectServerAsync(string ipAddress, int port)
        {
            try
            {
                if (!_tcpClient.Connected)
                {
                    Console.WriteLine("Attempting to connect to server at {0}:{1}", ipAddress, port);
                    await this._tcpClient.ConnectAsync(host: ipAddress, port: port);
                }

                Console.WriteLine("Connected to server at {0}:{1}", ipAddress, port);
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("SocketException: {0}", socketException.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: {0}", exception.Message);
            }
        }
    }
}
