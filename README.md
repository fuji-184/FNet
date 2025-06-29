# FNet

FNet is simple single threaded C# HTTP server library

> ⚠️ Experimental

## ✨ Features

- Non blocking server
- Minimal memory allocation
- Simple routing interface

## 📦 Installation

### 1. Manual download & install

1. Download the latest `.nupkg` from [Releases](https://github.com/fuji-184/FNet/releases)
2. Create a `packages/` folder inside your project (or your desired folder)
3. Move the `.nupkg` file into that folder
4. Run this command :

```bash
dotnet add package FNet --source ./packages --version 0.1.0
```

### 2. One liner shell install

Make sure you already have wget installed. Then run this command (change the link to the newest version) :

```bash
mkdir -p ./packages && cd packages && wget -O ./packages/FNet https://github.com/fuji-184/FNet/releases/download/v0.1.0/FNet.0.1.0.nupkg && dotnet add package FNet --source ./packages
```

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
