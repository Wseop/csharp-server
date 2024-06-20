using ServerCore.Network;
using ServerCore.Packet;

namespace DummyClient
{
    class ClientPacketHandler
    {
        public static ClientPacketHandler Instance { get; } = new ClientPacketHandler();

        private ClientPacketHandler()
        { }

        public void Init()
        {
            PacketManager.Instance.AddHandler(EPacketType.S_Enter, HandleS_Enter);
            PacketManager.Instance.AddHandler(EPacketType.S_Exit, HandleS_Exit);
        }

        public ArraySegment<byte> MakeC_Enter(string name)
        {
            PacketC_Enter cEnter = new PacketC_Enter();
            cEnter.Name = name;

            return PacketManager.Instance.MakePacket(EPacketType.C_Enter, cEnter);
        }

        private void HandleS_Enter(PacketSession session, MemoryStream payloadStream)
        {
            PacketS_Enter sEnter = ProtoBuf.Serializer.Deserialize<PacketS_Enter>(payloadStream);

            GameInstance.Instance.HandleEnter(sEnter.Success);
        }

        public ArraySegment<byte> MakeC_Exit(string name)
        {
            PacketC_Exit cExit = new PacketC_Exit();
            cExit.Name = name;

            return PacketManager.Instance.MakePacket(EPacketType.C_Exit, cExit);
        }

        private void HandleS_Exit(PacketSession session, MemoryStream payloadStream)
        {
            PacketS_Exit sExit = ProtoBuf.Serializer.Deserialize<PacketS_Exit>(payloadStream);

            GameInstance.Instance.HandleExit(sExit.Success);
        }
    }
}
