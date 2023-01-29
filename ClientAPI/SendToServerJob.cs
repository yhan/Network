using System.Net.Sockets;
using Common;
using Quartz;

public class SendToServerJob : IJob
{
    private static long loop = 0;
    private readonly string requestMessage;
    private readonly string region;

    public SendToServerJob(IConfiguration config)
    {
        requestMessage = Helper.StringWithSizeInMegaByte('c', 1);
        region = config["region"];
    }
    public async Task Execute(IJobExecutionContext context)
    {
        string message = $"{region}:{requestMessage}";
        string response = "";
        try
        {
            TcpClient client = new TcpClient("127.0.0.1", 8081);// Create a new connection  
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
