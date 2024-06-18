using ServerCore.Core;
using ServerCore.Network;
using ServerCore.Packet;
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

            PacketSession session = null;

            Connector connector = new Connector((socket) => {
                session = new PacketSession(socket);
                session.Start();
            });
            connector.Connect(ipEndPoint);

            int count = 0;

            while (true)
            {
                if (session == null)
                    continue;

                ArraySegment<byte> packetTest = ClientPacketHandler.Instance.MakeTest($"Test Message {count++}");
                session.Send(packetTest);

                Thread.Sleep(1000);
            }
        }
    }
}
