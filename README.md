# FNet

FNet is simple single threaded C# HTTP server library

> ⚠️ Experimental

## ✨ Features

- Non blocking server
- Minimal memory allocation
- Simple routing interface

## 📦 Installation

You can install FNet manually via the `.nupkg` file published in the [GitHub Releases](https://github.com/fuji-184/FNet/releases)

### Option 1: Manual download & install

1. Download the latest `.nupkg` from [Releases](https://github.com/fuji-184/FNet/releases)
2. Create a `packages/` folder inside your project (or your desired folder)
3. Move the `.nupkg` file into that folder
4. Run this :

```bash
dotnet add package FNet --source ./packages --version 0.1.0
```

### Option 2: One liner shell install

```bash
mkdir -p ./packages
curl -L -o ./packages/FNet.0.1.0.nupkg https://github.com/fuji-184/FNet/releases/download/v0.1.0/FNet.0.1.0.nupkg
dotnet add package FNet --source ./packages --version 0.1.0
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
