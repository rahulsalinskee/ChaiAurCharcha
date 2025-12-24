using ChaiAurCharcha.Server.Net.IO;
using System.Net.Sockets;

namespace ChaiAurCharcha.Server
{
    internal class Client
    {
        private PackageReader _packageReader;

        public string UserName { get; set; } = string.Empty;

        public Guid UID { get; set; }

        public TcpClient TcpClient { get; set; }

        public Client(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            UID = Guid.NewGuid();
            this._packageReader = new PackageReader(networkStream: TcpClient.GetStream());
        }

        internal async Task RunAsync()
        {
            try
            {
                /* 1. Read the initial OpCode (Connect) */
                byte opCode = _packageReader.ReadByte();

                /* 2. Read the Username properly using await */
                UserName = await _packageReader.ReadMessageAsync();

                Console.WriteLine($"{DateTime.Now}: {UserName} has joined the chatroom!");
                
                /* 3. NOW we broadcast the connection, because we have the username */
                await Program.BroadcastConnectionAsync();

                /* 4. Start processing messages */
                await ProcessPackageAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during client initialization: {ex.Message}");
                TcpClient.Close();
            }
        }

        private async Task ProcessPackageAsync()
        {
            while (true) 
            {
                try
                {
                    var opCode = _packageReader.ReadByte();
                    switch (opCode)
                    {
                        case 5:
                            var message = await _packageReader.ReadMessageAsync();
                            await Program.BroadcastMessageAsync(message: $"{DateTime.Now} - {UserName}: {message}");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{UID}: Disconnected - {exception.Message}");
                    await Program.BroadcastDisconnectAsync(UID: UID.ToString());
                    TcpClient.Close();
                    break;
                }
            }
        }
    }
}
