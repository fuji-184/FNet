using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace FNet
{
 public class SocketAsyncEventArgsPool
    {
        private readonly Stack<SocketAsyncEventArgs> _pool = new Stack<SocketAsyncEventArgs>();
        private readonly object _lock = new object();

        public SocketAsyncEventArgs Pop()
        {
            lock (_lock)
            {
                if (_pool.Count > 0)
                    return _pool.Pop();
            }

            // Create new if pool is empty
            var args = new SocketAsyncEventArgs();
            return args;
        }

        public void Push(SocketAsyncEventArgs args)
        {
            if (args == null) return;

            // Clean up before returning to pool
            args.AcceptSocket = null;
            args.UserToken = null;

            lock (_lock)
            {
                _pool.Push(args);
            }
        }
    }

}
