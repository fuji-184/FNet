using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace FNet
{
 public class HttpServer
    {
        private readonly Socket _listenSocket;
        private const int BufferSize = 8192;
        private readonly Router _router = new();
        private readonly SocketAsyncEventArgsPool _argsPool = new();
        private readonly BufferManager _bufferManager;

        public HttpServer(IPAddress ip, int port)
        {
            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _listenSocket.NoDelay = true;

            _listenSocket.Bind(new IPEndPoint(ip, port));
            _listenSocket.Listen(1000); // Increase backlog

            _bufferManager = new BufferManager(BufferSize);
        }

        public void Start() => StartAccept(null);

        public void Map(string path, RequestHandler handler) => _router.Register(path, handler);

        private void StartAccept(SocketAsyncEventArgs acceptArgs)
        {
            if (acceptArgs == null)
            {
                acceptArgs = new SocketAsyncEventArgs();
                acceptArgs.Completed += Accept_Completed;
            }
            else
            {
                acceptArgs.AcceptSocket = null;
            }

            if (!_listenSocket.AcceptAsync(acceptArgs))
                ProcessAccept(acceptArgs);
        }

        private void Accept_Completed(object sender, SocketAsyncEventArgs e) => ProcessAccept(e);

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            var clientSocket = e.AcceptSocket;
            if (clientSocket != null)
            {
                var readWriteArgs = _argsPool.Pop();
                readWriteArgs.UserToken = clientSocket;
                readWriteArgs.Completed += IO_Completed;

                _bufferManager.SetBuffer(readWriteArgs);

                clientSocket.NoDelay = true;

                if (!clientSocket.ReceiveAsync(readWriteArgs))
                    ProcessReceive(readWriteArgs);
            }

            StartAccept(e);
        }

        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            var clientSocket = (Socket)e.UserToken;

            if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success)
            {
                CloseConnection(clientSocket, e);
                return;
            }

            var buffer = e.Buffer.AsSpan(e.Offset, e.BytesTransferred);

            if (!HttpParser.TryParseRequest(buffer, out var request))
            {
                var response = new HttpResponse(e.Buffer.AsSpan(e.Offset));
                response.SetStatus(400, "Bad Request");
                response.SetContentType("text/plain");
                response.SetBody("Bad Request");

                e.SetBuffer(e.Offset, response.Length);
                if (!clientSocket.SendAsync(e))
                    ProcessSend(e);
                return;
            }

            var resp = new HttpResponse(e.Buffer.AsSpan(e.Offset));
            var handler = _router.Resolve(request.Path);

            if (handler == null)
            {
                resp.SetStatus(404, "Not Found");
                resp.SetContentType("text/plain");
                resp.SetBody("Not Found");
            }
            else
            {
                try
                {
                    handler(in request, ref resp);
                }
                catch
                {
                    resp = new HttpResponse(e.Buffer.AsSpan(e.Offset));
                    resp.SetStatus(500, "Internal Server Error");
                    resp.SetContentType("text/plain");
                    resp.SetBody("Internal Server Error");
                }
            }

            e.SetBuffer(e.Offset, resp.Length);
            if (!clientSocket.SendAsync(e))
                ProcessSend(e);
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            var clientSocket = (Socket)e.UserToken;

            if (e.SocketError == SocketError.Success)
            {
                e.SetBuffer(e.Offset, BufferSize);
                if (!clientSocket.ReceiveAsync(e))
                    ProcessReceive(e);
            }
            else
            {
                CloseConnection(clientSocket, e);
            }
        }

        private void CloseConnection(Socket clientSocket, SocketAsyncEventArgs e)
        {
            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
            }
            catch { }

            clientSocket.Close();

            _bufferManager.ReturnBuffer(e);

            e.Completed -= IO_Completed;

            _argsPool.Push(e);
        }
    }
}
