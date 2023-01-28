using System.Net;
using System.Net.Sockets;
using Common;

public class ServerService : BackgroundService
{
    static long loop = 0;
    private TcpListener listener;
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Server starting !");
        
        // IP Address to listen on. Loopback in this case
        IPAddress ipAddr = IPAddress.Loopback;
        // Port to listen on
        int port = 8081;
        // Create a network endpoint
        IPEndPoint ep = new IPEndPoint(ipAddr, port);
        // Create and start a TCP listener
        listener = new TcpListener(ep);
        listener.Start();

        Console.WriteLine("Server listening on: {0}:{1}", ep.Address, ep.Port);

        return base.StartAsync(cancellationToken);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                var sender = await listener.AcceptTcpClientAsync(stoppingToken);
                // streamToMessage - discussed later
                string request = await Helper.StreamToMessage(sender.GetStream());
                if (request != null)
                {
                    string responseMessage = MessageHandler(request);
                    SendMessage(responseMessage, sender);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    void SendMessage(string message, TcpClient client)
    {
        // messageToByteArray- discussed later
        byte[] bytes = Helper.MessageToByteArray(message);
        client.GetStream().Write(bytes, 0, bytes.Length);
    }

    string MessageHandler(string message)
    {
        Console.WriteLine($"[{loop++}] Received message, StrLength={message.Length} bytes={Helper.SizeInBytes(message) / 1024 / 1024} MB");
        return Helper.StringWithSizeInMegaByte('s', 1);//"Thank a lot for the message!";
    }
}
