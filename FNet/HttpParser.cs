using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace FNet
{
public static class HttpParser
    {
        private static readonly byte[] CRLF_CRLF = "\r\n\r\n"u8.ToArray();
        private static readonly byte SPACE = (byte)' ';
        private static readonly byte QUESTION = (byte)'?';

        public static bool TryParseRequest(ReadOnlySpan<byte> buffer, out HttpRequest request)
        {
            request = default;

            int headersEnd = buffer.IndexOf(CRLF_CRLF);
            if (headersEnd < 0) return false;

            var headersSpan = buffer.Slice(0, headersEnd);
            var bodySpan = buffer.Slice(headersEnd + 4);

            int firstLineEnd = headersSpan.IndexOf((byte)'\r');
            if (firstLineEnd < 0) return false;

            var requestLine = headersSpan.Slice(0, firstLineEnd);

            int methodEnd = requestLine.IndexOf(SPACE);
            if (methodEnd < 0) return false;

            var method = requestLine.Slice(0, methodEnd);

            int urlStart = methodEnd + 1;
            int urlEnd = requestLine.Slice(urlStart).IndexOf(SPACE);
            if (urlEnd < 0) return false;
            urlEnd += urlStart;

            var url = requestLine.Slice(urlStart, urlEnd - urlStart);

            int queryStart = url.IndexOf(QUESTION);
            ReadOnlySpan<byte> path, queryString;

            if (queryStart >= 0)
            {
                path = url.Slice(0, queryStart);
                queryString = url.Slice(queryStart + 1);
            }
            else
            {
                path = url;
                queryString = ReadOnlySpan<byte>.Empty;
            }

            var headers = headersSpan.Slice(firstLineEnd + 2);

            request = new HttpRequest(method, path, queryString, headers, bodySpan);
            return true;
        }
    }

    public delegate void RequestHandler(in HttpRequest req, ref HttpResponse res);
}
