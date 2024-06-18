using ServerCore.Network;
using ServerCore.Packet;

namespace Server
{
    class ServerPacketHandler
    {
        public static ServerPacketHandler Instance { get; } = new ServerPacketHandler();

        private ServerPacketHandler() 
        {
            PacketManager.Instance.AddHandler(EPacketType.Test, HandleTest);
        }

        public ArraySegment<byte> MakeTest(string msg)
        {
            PacketTest packetTest = new PacketTest();
            packetTest.Msg = msg;

            return PacketManager.Instance.MakePacket(EPacketType.Test, packetTest);
        }

        private void HandleTest(Session session, MemoryStream payloadStream)
        {
            PacketTest packetTest = ProtoBuf.Serializer.Deserialize<PacketTest>(payloadStream);

            ArraySegment<byte> reply = MakeTest(packetTest.Msg);
            session.Send(reply);
        }
    }
}
