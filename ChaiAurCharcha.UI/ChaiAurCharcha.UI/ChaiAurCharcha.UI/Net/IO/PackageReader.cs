using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChaiAurCharcha.UI.Net.IO
{
    public class PackageReader : BinaryReader
    {
        private NetworkStream _networkStream;

        public PackageReader(NetworkStream networkStream) : base(networkStream)
        {
            this._networkStream = networkStream;
        }

        internal string ReadMessage()
        {
            return Task.Run(() => ReadMessageAsync()).GetAwaiter().GetResult();
        }

        private async Task<string> ReadMessageAsync()
        {
            byte[] messageBuffer;
            var length = ReadInt32();
            messageBuffer = new byte[length];
            await _networkStream.ReadAsync(messageBuffer, 0, length);
            return Encoding.ASCII.GetString(messageBuffer);
        }
    }
}
