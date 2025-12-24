using ChaiAurCharcha.UI.Net.IO;
using System.Net.Sockets;

namespace ChaiAurCharcha.UI.Net
{
    public class Server
    {
        private TcpClient _tcpClient;

        private const string LOCAL_SERVER_IP = "127.0.0.1";
        private const int PORT = 7891;

        public PackageReader PackageReader { get; set; }

        public event Action connectedEvent;
        public event Action messageReceivedEvent;
        public event Action userDisconnectedEvent;

        public Server()
        {
            this._tcpClient = new TcpClient();
        }

        public void ConnectToServer(string userName)
        {
            Task.Run(() => ConnectServerAsync(ipAddress: LOCAL_SERVER_IP, port: PORT, userName: userName));
        }

        /// <summary>
        /// Attempts to connect to a server at a given IP address and port with a given username.
        /// </summary>
        /// <param name="ipAddress">The IP address of the server to connect to.</param>
        /// <param name="port">The port number of the server to connect to.</param>
        /// <param name="userName">The username to use when connecting to the server.</param>
        private async Task ConnectServerAsync(string ipAddress, int port, string userName)
        {
            try
            {
                if (!_tcpClient.Connected)
                {
                    Console.WriteLine("Attempting to connect to server at {0}:{1}", ipAddress, port);
                    await this._tcpClient.ConnectAsync(host: ipAddress, port: port);
                    PackageReader = new PackageReader(networkStream: this._tcpClient.GetStream());

                    if (!string.IsNullOrEmpty(userName))
                    {
                        PackageBuilder connectPackage = new();
                        connectPackage.WriteOpCode(opCode: 0);
                        await connectPackage.WriteMessageAsync(message: userName);
                        await this._tcpClient.Client.SendAsync(buffer: connectPackage.GetPacketBytes());
                    }

                    ReadPackage();
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

        private void ReadPackage()
        {
            /* Offload the data into a different thread */
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var opCode = PackageReader.ReadByte();

                        switch (opCode)
                        {
                            case 1:
                                {
                                    //var userName = PackageReader.ReadString();
                                    //var uid = PackageReader.ReadString();
                                    //Console.WriteLine("User connected: {0} (UID: {1})", userName, uid);
                                    /* Trigger the connected event */
                                    connectedEvent?.Invoke();
                                }
                                break;

                            case 5:
                                {
                                    messageReceivedEvent?.Invoke();
                                }
                                break;

                            case 10:
                                {
                                    userDisconnectedEvent?.Invoke();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error reading package: {0}", ex.Message);
                        break;
                    }
                }
            });
        }

        public async Task SendMessageToServerAsync(string message)
        {
            PackageBuilder packageBuilder = new();
            packageBuilder.WriteOpCode(opCode: 5);
            packageBuilder.WriteMessage(message: message);

            await _tcpClient.Client.SendAsync(buffer: packageBuilder.GetPacketBytes());
        }
    }
}
