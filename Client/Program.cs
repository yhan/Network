using System.Net.Sockets;
using Common;

long loop = 0;
string requestMessage = new string(Enumerable.Repeat('a', 1024 * 1024).ToArray()) ;
while (true)
{
    string responseMessage = SendMessage(requestMessage);
    Console.WriteLine($"[{loop++}] {responseMessage}");
}

string SendMessage(string message)
{
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
            response = Helper.StreamToMessage(stream);
        }
        client.Close();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
    return response;
}
