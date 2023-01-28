using Quartz;

public class SendToServerJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"hello {DateTime.Now}");
        return Task.CompletedTask;
    }
}
