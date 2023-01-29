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
        try
        {
            TcpClient client = new TcpClient(targetServer.Address, targetServer.Port);// Create a new connection 
            client.NoDelay = true;// please check TcpClient for more optimization
            byte[] messageBytes = Helper.MessageToByteArray(message);

            string reply = string.Empty, replyFromRegion = string.Empty;
            using (NetworkStream stream = client.GetStream())
            {
                stream.Write(messageBytes, 0, messageBytes.Length);

                // Message sent!  Wait for the response stream of bytes...
                reply = await Helper.StreamToMessage(stream);
                replyFromRegion = reply.Split(':')[0];
            }
            client.Close();
            Console.WriteLine($"[{loop++}][{config.CurrentRegion}][AS CLIENT] Received reply from [{replyFromRegion}] StrLength={reply.Length} bytes={Helper.SizeInBytes(reply) / 1024 / 1024}  MB");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
    }
}