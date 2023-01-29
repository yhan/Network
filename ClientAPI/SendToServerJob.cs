using System.Diagnostics;
using System.Net.Sockets;
using Common;
using Microsoft.Extensions.Options;
using Quartz;

namespace ClientAPI;

public class SendToServerJob : IJob
{
    private readonly Config config;
    private static long loop = 0;
    private readonly string requestMessage;

    public SendToServerJob(IOptions<Config> appConfig)
    {
        config = appConfig.Value;
        Debug.Assert(this.config.Client.TargetServers is { Length: > 0 });
        
        this.config = config;
        requestMessage = Helper.StringWithSizeInMegaByte('c', 1);
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        foreach (var targetServer in config.Client.TargetServers)
        {
            await RoundTripToServer(targetServer);
        }
    }
    private async Task RoundTripToServer(TargetServer targetServer)
    {
        string message = $"{config.CurrentRegion}:{requestMessage}";
        string response = "";
        try
        {
            TcpClient client = new TcpClient(targetServer.Address, targetServer.Port);// Create a new connection   // TODO move to Config
            client.NoDelay = true;// please check TcpClient for more optimization
            // messageToByteArray- discussed later
            byte[] messageBytes = Helper.MessageToByteArray(message);

            using (NetworkStream stream = client.GetStream())
            {
                stream.Write(messageBytes, 0, messageBytes.Length);

                // Message sent!  Wait for the response stream of bytes...
                // streamToMessage - discussed later
                response = await Helper.StreamToMessage(stream);
            }
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        string reply = response;
        Console.WriteLine($"[{loop++}] response StrLength={reply.Length} bytes={Helper.SizeInBytes(reply) / 1024 / 1024}  MB");
    }
}