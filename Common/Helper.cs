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
        return new string(Enumerable.Repeat(c, unitOfMegaBytes * 2 * 1024 * 1024).ToArray());
    }

    public static string StreamToMessage(Stream stream)
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
    }
    public static int SizeInBytes(string s)
    {
        return 20 + (s.Length / 2) * 4;
    }

}
