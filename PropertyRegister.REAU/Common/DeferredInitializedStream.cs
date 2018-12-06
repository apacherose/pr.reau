using CNSys.Data;
using PropertyRegister.REAU.Persistence;
using System;
using System.IO;

namespace PropertyRegister.REAU.Common
{
    public class DeferredDbStream : Stream
    {
        private bool _isInnerStreamInitialized = false;
        private readonly Func<Tuple<DbContext, Stream>> _streamFactory;
        private Stream _innerStream;
        private DbContext _dbContext;

        public DeferredDbStream(Func<Tuple<DbContext, Stream>> streamFactory)
        {
            _streamFactory = streamFactory ?? throw new ArgumentNullException(nameof(streamFactory));
        }
        
        #region Overriden Methods

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            EnsureInnerStreamInitialized();

            return _innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            EnsureInnerStreamInitialized();
            return _innerStream.EndRead(asyncResult);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            EnsureInnerStreamInitialized();
            return _innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            EnsureInnerStreamInitialized();
            _innerStream.EndWrite(asyncResult);
        }

        public override bool CanRead
        {
            get
            {
                EnsureInnerStreamInitialized();
                return _innerStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                EnsureInnerStreamInitialized();
                return _innerStream.CanSeek;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                EnsureInnerStreamInitialized();
                return _innerStream.CanTimeout;
            }
        }

        public override bool CanWrite
        {
            get
            {
                EnsureInnerStreamInitialized();
                return _innerStream.CanWrite;
            }
        }

        public override void Close()
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_dbContext != null)
            {
                _dbContext.Complete();
                _dbContext.Dispose();
                _dbContext = null;
            }

            if (_innerStream != null)
            {
                _innerStream.Dispose();
                _innerStream = null;
            }
        }

        public override long Length
        {
            get
            {
                EnsureInnerStreamInitialized();
                return _innerStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                EnsureInnerStreamInitialized();
                return _innerStream.Position;
            }
            set
            {
                EnsureInnerStreamInitialized();
                _innerStream.Position = value;
            }
        }

        public override int ReadTimeout
        {
            get
            {
                EnsureInnerStreamInitialized();
                return _innerStream.ReadTimeout;
            }
            set
            {
                EnsureInnerStreamInitialized();
                _innerStream.ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                EnsureInnerStreamInitialized();
                return _innerStream.WriteTimeout;
            }
            set
            {
                EnsureInnerStreamInitialized();
                _innerStream.WriteTimeout = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            EnsureInnerStreamInitialized();
            return _innerStream.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            EnsureInnerStreamInitialized();
            return _innerStream.ReadByte();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            EnsureInnerStreamInitialized();
            _innerStream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            EnsureInnerStreamInitialized();
            _innerStream.WriteByte(value);
        }

        public override void SetLength(long value)
        {
            EnsureInnerStreamInitialized();
            _innerStream.SetLength(value);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            EnsureInnerStreamInitialized();
            return _innerStream.Seek(offset, origin);
        }

        public override void Flush()
        {
            EnsureInnerStreamInitialized();
            _innerStream.Flush();
        }

        #endregion

        #region Protected Interface

        protected virtual void InitializeInnerStream()
        {
            _isInnerStreamInitialized = true;
            (_dbContext, _innerStream) = _streamFactory();
        }

        protected void EnsureInnerStreamInitialized()
        {
            if (!_isInnerStreamInitialized)
            {
                InitializeInnerStream();
            }
        }

        #endregion
    }
}
