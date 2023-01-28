using System.Net.Sockets;
using System.Text;

namespace Common;

public static class Helper
{
    // using UTF8 encoding for the messages
    static Encoding encoding = Encoding.UTF8;
    public static byte[] MessageToByteArray(string message)
    {
        // get the size of original message
        byte[] messageBytes = encoding.GetBytes(message);
        int messageSize = messageBytes.Length;
        // add content length bytes to the original size
        int completeSize = messageSize + 4;
        // create a buffer of the size of the complete message size
        byte[] completeMsg = new byte[completeSize];

        // convert message size to bytes
        byte[] sizeBytes = BitConverter.GetBytes(messageSize);
        // copy the size bytes and the message bytes to our overall message to be sent 
        sizeBytes.CopyTo(completeMsg, 0);
        messageBytes.CopyTo(completeMsg, 4);
        return completeMsg;
    }

    public static string StringWithSizeInMegaByte(char c, int unitOfMegaBytes)
    {
        return new string(Enumerable.Repeat(c, unitOfMegaBytes * 1024 * 1024 / 2).ToArray());
    }

    public static async Task<string> StreamToMessage(Stream stream)
    {
        var task = Task.Run(()=>
        {
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
    public static int SizeInBytes(string s)
    {
        return 20 + (s.Length / 2) * 4;
    }


    public static async Task<string> ClientSendMessage(string message)
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
                response = await Helper.StreamToMessage(stream);
            }
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return response;
    }

}
