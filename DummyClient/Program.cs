using ServerCore.Core;
using System.Net;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(host);
            IPAddress ipAddress = hostEntry.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 7777);

            Connector connector = new Connector((socket) => { Console.WriteLine("Connected to Server."); });
            connector.Connect(ipEndPoint);

            while (true) ;
        }
    }
}
