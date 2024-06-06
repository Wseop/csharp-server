using ServerCore.Core;
using System.Net;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(host);
            IPAddress ipAddress = hostEntry.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 7777);

            Listener listener = new Listener(ipEndPoint, (socket) => { Console.WriteLine("Client Connected."); });
            listener.StartAccept();

            while (true) ;
        }
    }
}
