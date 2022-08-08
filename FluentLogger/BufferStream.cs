using System;
using System.IO;

namespace FluentLogger
{
    public class BufferStream : Stream, IDisposable
    {
        private readonly Stream _stream;
        private MemoryStream _ms = new MemoryStream();

        public BufferStream(Stream stream)
        {
            _stream = stream;
        }

        public override bool CanRead => _ms.CanRead;

        public override bool CanSeek => _ms.CanSeek;

        public override bool CanWrite => _ms.CanWrite;

        public override long Length => _ms.Length;

        public override long Position { get => _ms.Position; set => _ms.Position = value; }

        public new void Dispose()
        {
            Flush();
            _stream.Dispose();
            _ms.Dispose();
        }

        public override void Flush()
        {
            _ms.Seek(0, SeekOrigin.Begin);
            _ms.CopyTo(_stream);
            _ms.SetLength(0);
            _stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _ms.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _ms.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _ms.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _ms.Write(buffer, offset, count);
            if (_ms.Length > 10000)
                this.Flush();
        }
    }
}
