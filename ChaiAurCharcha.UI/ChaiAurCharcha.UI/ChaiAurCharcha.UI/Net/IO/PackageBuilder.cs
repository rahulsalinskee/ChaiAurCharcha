using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ChaiAurCharcha.UI.Net.IO
{
    internal class PackageBuilder : IDisposable
    {
        private MemoryStream _memoryStream;
        private bool _disposed;

        public PackageBuilder()
        {
            this._memoryStream = new();
        }

        internal void WriteOpCode(byte opCode)
        {
            ThrowIfDisposed();
            this._memoryStream.WriteByte(opCode);
        }

        internal async Task WriteMessageAsync(string message)
        {
            ThrowIfDisposed();
            
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            int messageLength = message.Length;

            await _memoryStream.WriteAsync(BitConverter.GetBytes(messageLength));
            await _memoryStream.WriteAsync(Encoding.UTF8.GetBytes(message));
        }

        internal void WriteMessage(string message)
        {
            ThrowIfDisposed();
            
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            int messageLength = message.Length;

            _memoryStream.Write(BitConverter.GetBytes(messageLength));
            _memoryStream.Write(Encoding.UTF8.GetBytes(message));
        }

        internal byte[] GetPacketBytes()
        {
            ThrowIfDisposed();
            return this._memoryStream.ToArray();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _memoryStream?.Dispose();
            }

            _disposed = true;
        }

        ~PackageBuilder()
        {
            Dispose(false);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PackageBuilder));
        }
    }
}
