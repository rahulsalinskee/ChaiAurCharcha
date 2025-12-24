using ChaiAurCharcha.Server.Net.IO;
using System.Net;
using System.Net.Sockets;

namespace ChaiAurCharcha.Server
{
    internal class Program
    {
        private static TcpListener _tcpListener = default!;
        private static List<Client>? _users;
        private static readonly object _lock = new();
        private const string LOCAL_HOST_SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 7891;

        async static Task Main(string[] args)
        {
            _users = new();
            _tcpListener = new(localaddr: IPAddress.Parse(ipString: LOCAL_HOST_SERVER_IP), port: SERVER_PORT);
            _tcpListener.Start();

            while (true)
            {
                var tcpClient = new Client(tcpClient: _tcpListener.AcceptTcpClient());
                lock (_lock)
                {
                    _users?.Add(tcpClient);
                }
                
                /* Run the client without awaiting - it handles its own async operations */
                _ = Task.Run(async () => await tcpClient.RunAsync());
            }
        }

        internal static async Task BroadcastConnectionAsync()
        {
            List<Client>? usersCopy;
            lock (_lock)
            {
                /* Create a copy to iterate safely */
                usersCopy = new(_users);
            }

            foreach (var user in usersCopy!)
            {
                foreach (var eachUser in usersCopy!)
                {
                    using (PackageBuilder broadcastPackage = new())
                    {
                        broadcastPackage.WriteOpCode(opCode: 1);
                        await broadcastPackage.WriteMessageAsync(message: eachUser.UserName);
                        await broadcastPackage.WriteMessageAsync(message: eachUser.UID.ToString());
                        await user.TcpClient.Client.SendAsync(buffer: broadcastPackage.GetPacketBytes());
                    }
                }
            }
        }

        internal async static Task BroadcastMessageAsync(string message)
        {
            List<Client>? usersCopy;
            lock (_lock)
            {
                usersCopy = new(_users);
            }

            foreach (var user in usersCopy!)
            {
                using (PackageBuilder broadcastPackage = new())
                {
                    broadcastPackage.WriteOpCode(opCode: 5);
                    await broadcastPackage.WriteMessageAsync(message: message);
                    await user.TcpClient.Client.SendAsync(buffer: broadcastPackage.GetPacketBytes());
                }
            }
        }

        internal async static Task BroadcastDisconnectAsync(string UID)
        {
            Client disconnectedUser = null;
            lock (_lock)
            {
                disconnectedUser = _users?.Where(user => user.UID.ToString() == UID).FirstOrDefault();
                _users?.Remove(disconnectedUser);
            }

            List<Client>? usersCopy;
            lock (_lock)
            {
                usersCopy = new(_users);
            }

            foreach (var user in usersCopy!)
            {
                using (PackageBuilder broadcastPackageBuilder = new())
                {
                    broadcastPackageBuilder.WriteOpCode(opCode: 10);
                    await broadcastPackageBuilder.WriteMessageAsync(message: UID);
                    await user.TcpClient.Client.SendAsync(buffer: broadcastPackageBuilder.GetPacketBytes());
                }
            }

            await BroadcastMessageAsync(message: $"{disconnectedUser?.UserName} has left the chatroom.");
        }
    }
}
