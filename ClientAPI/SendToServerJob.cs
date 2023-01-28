using Common;
using Quartz;

public class SendToServerJob : IJob
{
    private static long loop = 0;
    private readonly string requestMessage;

    public SendToServerJob()
    {
        requestMessage = Helper.StringWithSizeInMegaByte('c', 1);
    }
    public async Task Execute(IJobExecutionContext context)
    {
        string reply = await Helper.ClientSendMessage(requestMessage);
        Console.WriteLine($"[{loop++}] response StrLength={reply.Length} bytes={Helper.SizeInBytes(reply) / 1024 / 1024}  MB");
    }
}
