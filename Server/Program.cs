using System.Net;
using System.Net.Sockets;
using Common;

Console.WriteLine("Server starting !");

// IP Address to listen on. Loopback in this case
IPAddress ipAddr = IPAddress.Loopback;
// Port to listen on
int port = 8081;
// Create a network endpoint
IPEndPoint ep = new IPEndPoint(ipAddr, port);
// Create and start a TCP listener
TcpListener listener = new TcpListener(ep);
listener.Start();

Console.WriteLine("Server listening on: {0}:{1}", ep.Address, ep.Port);

// keep running
while (true)
{
    var sender = listener.AcceptTcpClient();
    // streamToMessage - discussed later
    string request = Helper.streamToMessage(sender.GetStream());
    if (request != null)
    {
        string responseMessage = MessageHandler(request);
        sendMessage(responseMessage, sender);
    }
}


void sendMessage(string message, TcpClient client)
{
    // messageToByteArray- discussed later
    byte[] bytes = Helper.messageToByteArray(message);
    client.GetStream().Write(bytes, 0, bytes.Length);
}

string MessageHandler(string message)
{
    Console.WriteLine("Received message: " + message);
    return "Thank a lot for the message!";
}
