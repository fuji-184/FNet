using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace FNet
{
 public ref struct HttpResponse
    {
        public Span<byte> Buffer;
        public int Length;
        private int _cursor;
        private bool _headersComplete;

        public HttpResponse(Span<byte> buffer)
        {
            Buffer = buffer;
            Length = 0;
            _cursor = 0;
            _headersComplete = false;
        }

        public void SetStatus(int statusCode, ReadOnlySpan<char> reasonPhrase = default)
        {
            if (reasonPhrase.IsEmpty)
            {
                reasonPhrase = statusCode switch
                {
                    200 => "OK",
                    404 => "Not Found",
                    500 => "Internal Server Error",
                    _ => "Unknown"
                };
            }

            _cursor = 0;
            _cursor += WriteAscii($"HTTP/1.1 {statusCode} ");
            _cursor += WriteAscii(reasonPhrase);
            _cursor += WriteAscii("\r\n");
        }

        public void SetContentType(ReadOnlySpan<char> contentType)
        {
            _cursor += WriteAscii("Content-Type: ");
            _cursor += WriteAscii(contentType);
            _cursor += WriteAscii("\r\n");
        }

        public void AddHeader(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
        {
            _cursor += WriteAscii(name);
            _cursor += WriteAscii(": ");
            _cursor += WriteAscii(value);
            _cursor += WriteAscii("\r\n");
        }

        public void SetBody(ReadOnlySpan<char> body)
        {
            _cursor += WriteAscii($"Content-Length: {Encoding.UTF8.GetByteCount(body)}\r\n");

            _cursor += WriteAscii("\r\n");

            _cursor += Encoding.UTF8.GetBytes(body, Buffer.Slice(_cursor));

            Length = _cursor;
            _headersComplete = true;
        }

        public void SetBody(ReadOnlySpan<byte> body)
        {
            _cursor += WriteAscii($"Content-Length: {body.Length}\r\n");

            _cursor += WriteAscii("\r\n");

            body.CopyTo(Buffer.Slice(_cursor));
            _cursor += body.Length;

            Length = _cursor;
            _headersComplete = true;
        }

        public void Complete()
        {
            if (!_headersComplete)
            {
                _cursor += WriteAscii("Content-Length: 0\r\n\r\n");
                Length = _cursor;
                _headersComplete = true;
            }
        }

        private int WriteAscii(ReadOnlySpan<char> text)
        {
            return Encoding.ASCII.GetBytes(text, Buffer.Slice(_cursor));
        }
    }

}
