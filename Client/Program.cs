using System.Net.Sockets;
using Common;

string requestMessage = "Please process this message!";
while (true)
{
    string responseMessage = sendMessage(requestMessage);
    Console.WriteLine(responseMessage);
}

string sendMessage(string message)
{
    string response = "";
    try
    {
        TcpClient client = new TcpClient("127.0.0.1", 8081);// Create a new connection  
        client.NoDelay = true;// please check TcpClient for more optimization
        // messageToByteArray- discussed later
        byte[] messageBytes = Helper.messageToByteArray(message);

        using (NetworkStream stream = client.GetStream())
        {
            stream.Write(messageBytes, 0, messageBytes.Length);

            // Message sent!  Wait for the response stream of bytes...
            // streamToMessage - discussed later
            response = Helper.streamToMessage(stream);
        }
        client.Close();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
    return response;
}
