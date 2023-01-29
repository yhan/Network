namespace Common;
/*
 "Config": {
    "Client":{
      "CurrentRegion": "HK",
      "TargetServers": [
        {
          "Address": "127.0.0.1",
          "Port": 8081
        }
      ],
      "ClientSendFreq": "0/10 * * * * ?"
    },
    "Server": {
      "ListenOnPort": 8082
    }  
  }
*/
public class ClientConfig
{
    public TargetServer[] TargetServers { get; set; }
    public string SendFreq { get; set; }
}

public class ServerConfig
{
    public int? ListenOnPort { get; set; }
}

public class Config
{
    public string CurrentRegion { get; set; }

    public ClientConfig Client
    {
        get;
        set;
    }
    public ServerConfig Server
    {
        get;
        set;
    }
}

public class TargetServer
{
    public string Address { get; set; }
    public int Port { get; set; }
}