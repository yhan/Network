https://techitmore.com/dotnet/socket-programming-tcp-client-and-server-in-net-c/

```appsettings
// start on port
"Kestrel": {
        "EndPoints": {
        "Http": {
            "Url": "http://0.0.0.0:5002"
        }
    }
}
```

dotnet publish -r win-x64 /p:PublishSingleFile=true -o=.\bin\publish



EU(server)  <---->  US (client) (client)
                                   /\
                                    |
                                   \/
            <---->  HK (client) (server)

> EU only as server
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Config": {
    "CurrentRegion": "EU",
    "Server": {
      "ListenOnPort": 8081
    }
  },
  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "http://0.0.0.0:5002"
      }
    }
  }
}
```

every 10 sec = "0/10 * * * * ?"
every sec = "* * * ? * *"
every 5Min = "0 0/5 * * * ?"


