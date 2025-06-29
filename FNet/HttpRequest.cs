using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace FNet
{
 public readonly ref struct HttpRequest
    {
        public readonly ReadOnlySpan<byte> Method;
        public readonly ReadOnlySpan<byte> Path;
        public readonly ReadOnlySpan<byte> QueryString;
        public readonly ReadOnlySpan<byte> Headers;
        public readonly ReadOnlySpan<byte> Body;

        public HttpRequest(ReadOnlySpan<byte> method, ReadOnlySpan<byte> path,
                          ReadOnlySpan<byte> queryString, ReadOnlySpan<byte> headers,
                          ReadOnlySpan<byte> body)
        {
            Method = method;
            Path = path;
            QueryString = queryString;
            Headers = headers;
            Body = body;
        }

        public string GetPath() => Encoding.ASCII.GetString(Path);

        public string GetMethod() => Encoding.ASCII.GetString(Method);
    }

}
