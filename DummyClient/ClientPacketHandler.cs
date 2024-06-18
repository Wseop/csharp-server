using ServerCore.Network;
using ServerCore.Packet;

namespace DummyClient
{
    class ClientPacketHandler
    {
        public static ClientPacketHandler Instance { get; } = new ClientPacketHandler();

        private ClientPacketHandler()
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

            Console.WriteLine($"Server Test Message : {packetTest.Msg}");
        }
    }
}
