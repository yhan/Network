using System.Net.Sockets;
using Common;



long loop = 0;
string requestMessage = Helper.StringWithSizeInMegaByte('c', 1) ;
while (true)
{
    string reply = await Helper.ClientSendMessage(requestMessage);
    Console.WriteLine($"[{loop++}] response StrLength={reply.Length} bytes={Helper.SizeInBytes(reply) / 1024 / 1024}  MB");
}
