using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Common;



public class ServerService : BackgroundService
{
    private readonly Perf perf;
    private readonly Config config;
    static long loop = 0;
    private TcpListener listener;
    static Encoding encoding = Encoding.UTF8;

    public ServerService(Perf perf, IOptions<Config> appConfig)
    {
        this.perf = perf;
        this.config = appConfig.Value;
        
        Debug.Assert(config.Server.ListenOnPort.HasValue);
    }
    
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Server starting !");
        
        // IP Address to listen on. Loopback in this case
        IPAddress ipAddr = IPAddress.Loopback; // TODO IPv6 ?
        // Port to listen on
        int port = config.Server.ListenOnPort.Value; // TODO move to Config
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
                perf.RecordValue(Stopwatch.GetTimestamp() - inboundStart, region, config.CurrentRegion);
                
                //OUTBOUND
                long outBoundStart = Stopwatch.GetTimestamp();
                string responseMessage = Helper.StringWithSizeInMegaByte('s', 1);
                SendMessage(responseMessage, sender);
                perf.RecordValue(Stopwatch.GetTimestamp() - outBoundStart, config.CurrentRegion, region);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private static async Task<string> StreamToMessage(Stream stream)
    {
        var tokenSource2 = new CancellationTokenSource();
        CancellationToken ct = tokenSource2.Token;
        tokenSource2.CancelAfter(1_000);

        var task = Task.Run(() => {
            // Were we already canceled?
            ct.ThrowIfCancellationRequested();
            
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
        }, ct);

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