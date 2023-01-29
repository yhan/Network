namespace UnitTests;

public class Tests
{
    [SetUp]
    public void Setup() {}

    [Test]
    public async Task Test1()
    {
        var tokenSource2 = new CancellationTokenSource();
        CancellationToken ct = tokenSource2.Token;
        var task = Task.Run(() => {
            Thread.Sleep(5000);
            return 42;
        },
        ct);
        if (await Task.WhenAny(task, Task.Delay(1000, ct)) == task)
        {
            TestContext.WriteLine(task.Result);
        }
        else
        {
            tokenSource2.Cancel();
            TestContext.WriteLine("Cancelled !!!!! ");
            TestContext.WriteLine("timeout");
        }
    }
}
