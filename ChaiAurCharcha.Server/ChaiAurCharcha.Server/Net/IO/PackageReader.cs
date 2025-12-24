using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChaiAurCharcha.Server.Net.IO
{
    internal class PackageReader : BinaryReader
    {
        private NetworkStream _networkStream;

        public PackageReader(NetworkStream networkStream) : base(networkStream)
        {
            this._networkStream = networkStream;
        }

        internal async Task<string> ReadMessageAsync()
        {
            byte[] messageBuffer;
            var length = ReadInt32();
            messageBuffer = new byte[length];
            await _networkStream.ReadAsync(messageBuffer, 0, length);
            return Encoding.UTF8.GetString(messageBuffer);
        }
    }
}
