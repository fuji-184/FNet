### ✅ `README.md`

````markdown
# FNet

FNet is a high-performance, asynchronous HTTP server library for .NET designed to handle unlimited concurrent connections using a socket pooling model. It is ideal for lightweight APIs, testing tools, and high-throughput scenarios.

> ⚠️ Currently experimental. API may change.

## ✨ Features

- Asynchronous non blocking server
- Minimal memory allocation with socket pooling
- Simple routing interface

## 📦 Installation

If you have [GitHub Packages configured](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry):

```bash
dotnet add package FNet --version 0.1.0
```
````

Or reference the `.nupkg` directly from [GitHub Packages](https://github.com/users/fuji-184/packages?tab=packages).

## 🚀 Quick Start

```csharp
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
            res.SetBody("<h1>Hello!</h1>");
        });

        server.Map("/api/test", (in HttpRequest req, ref HttpResponse res) =>
        {
            res.SetStatus(200);
            res.SetContentType("application/json");
            res.SetBody("{\"message\":\"Hello API\",\"method\":\"" + req.GetMethod() + "\"}");
        });

        Console.WriteLine("Server starting on http://localhost:8080");
        server.Start();

        Console.WriteLine("Press any key to stop...");
        Console.ReadKey();
    }
}
```

## 🛠 Requirements

- [.NET 7.0+](https://dotnet.microsoft.com/) (tested on .NET 9)
- GitHub package access (if using GitHub Registry)
