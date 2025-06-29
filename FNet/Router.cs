using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace FNet
{
 public class Router
    {
        private readonly Dictionary<string, RequestHandler> _routes = new();

        public void Register(string path, RequestHandler handler)
        {
            _routes[path] = handler;
        }

        public RequestHandler Resolve(ReadOnlySpan<byte> path)
        {
            var pathStr = Encoding.ASCII.GetString(path);
            return _routes.TryGetValue(pathStr, out var handler) ? handler : null;
        }
    }

}
