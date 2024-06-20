using ServerCore.Core;
using ServerCore.Network;
using System.Net;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerPacketHandler.Instance.Init();

            string host = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(host);
            IPAddress ipAddress = hostEntry.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 7777);

            Listener listener = new Listener(ipEndPoint, (socket) => {
                PacketSession session = SessionManager.Instance.CreateSession(socket);
                session.Start();
            });
            listener.StartAccept();

            while (true) ;
        }
    }
}
