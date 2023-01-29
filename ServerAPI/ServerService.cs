using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common;

public class ServerService : BackgroundService
{
    private readonly Perf perf;
    static long loop = 0;
    private TcpListener listener;
    static Encoding encoding = Encoding.UTF8;
    private readonly string thisRegion;

    public ServerService(Perf perf, IConfiguration config)
    {
        this.perf = perf;
        thisRegion = config["region"];
    }
    
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

                //INBOUND
                long inboundStart = Stopwatch.GetTimestamp();
                string request = await StreamToMessage(sender.GetStream());
                var region = request.Split(':')[0];
                Console.WriteLine($"[{loop++}][{region}] Received message, StrLength={request.Length} bytes={Helper.SizeInBytes(request) / 1024 / 1024} MB");
                perf.RecordValue(Stopwatch.GetTimestamp() - inboundStart, region, thisRegion);
                
                //OUTBOUND
                long outBoundStart = Stopwatch.GetTimestamp();
                string responseMessage = Helper.StringWithSizeInMegaByte('s', 1);
                SendMessage(responseMessage, sender);
                perf.RecordValue(Stopwatch.GetTimestamp() - outBoundStart, thisRegion, region);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public static async Task<string> StreamToMessage(Stream stream)
    {
        var task = Task.Run(() => {
            // size bytes have been fixed to 4
            byte[] sizeBytes = new byte[4];
            // read the content length
            stream.Read(sizeBytes, 0, 4);
            int messageSize = BitConverter.ToInt32(sizeBytes, 0);
            // create a buffer of the content length size and read from the stream
            byte[] messageBytes = new byte[messageSize];
            stream.Read(messageBytes, 0, messageSize);
            // convert message byte array to the message string using the encoding
            string message = encoding.GetString(messageBytes);
            return message;
        });

        if (await Task.WhenAny(task, Task.Delay(1000)) == task)
        {
            return task.Result;
        }
        else
        {
            return "timeout";
        }
    }
    

    void SendMessage(string message, TcpClient client)
    {
        // messageToByteArray- discussed later
        byte[] bytes = Helper.MessageToByteArray(message);
        client.GetStream().Write(bytes, 0, bytes.Length);
    }

}
