using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FNet
{
 public class BufferManager
    {
        private readonly int _bufferSize;
        private readonly object _lock = new object();
        private readonly Queue<byte[]> _bufferPool = new Queue<byte[]>();

        public BufferManager(int bufferSize)
        {
            _bufferSize = bufferSize;
        }

        public void SetBuffer(SocketAsyncEventArgs args)
        {
            byte[] buffer;
            lock (_lock)
            {
                if (_bufferPool.Count > 0)
                {
                    buffer = _bufferPool.Dequeue();
                }
                else
                {
                    buffer = new byte[_bufferSize];
                }
            }
            args.SetBuffer(buffer, 0, _bufferSize);
        }

        public void ReturnBuffer(SocketAsyncEventArgs args)
        {
            if (args.Buffer != null)
            {
                lock (_lock)
                {
                    _bufferPool.Enqueue(args.Buffer);
                }
                args.SetBuffer(null, 0, 0);
            }
        }
    }

}
