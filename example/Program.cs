using FNet;
using System;
using System.Net;


public class Program
    {
        public static void Main()
        {
            var server = new HttpServer(IPAddress.Any, 8080);

            server.Map("/", (in HttpRequest req, ref HttpResponse res) =>
            {
                res.SetStatus(200);
                res.SetContentType("text/html");
                res.SetBody("<h1>Hello, World!</h1><p>Unlimited connections supported!</p>");
            });

            server.Map("/api/test", (in HttpRequest req, ref HttpResponse res) =>
            {
                res.SetStatus(200);
                res.SetContentType("application/json");
                res.SetBody("{\"message\":\"Hello API\",\"method\":\"" + req.GetMethod() + "\",\"unlimited\":true}");
            });

            server.Map("/ping", (in HttpRequest req, ref HttpResponse res) =>
            {
                res.SetStatus(200);
                res.SetContentType("text/plain");
                res.SetBody("pong");
            });

            server.Map("/stress", (in HttpRequest req, ref HttpResponse res) =>
            {
                res.SetStatus(200);
                res.SetContentType("text/plain");
                res.SetBody($"Connection handled at {DateTime.Now:HH:mm:ss.fff}");
            });

            Console.WriteLine("Server starting on http://localhost:8080");
            server.Start();

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }
    }

