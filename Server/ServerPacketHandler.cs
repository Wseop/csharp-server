using ServerCore.Network;
using ServerCore.Packet;

namespace Server
{
    class ServerPacketHandler
    {
        public static ServerPacketHandler Instance { get; } = new ServerPacketHandler();

        private ServerPacketHandler() 
        { }

        public void Init()
        {
            PacketManager.Instance.AddHandler(EPacketType.C_Enter, HandleC_Enter);
            PacketManager.Instance.AddHandler(EPacketType.C_Exit, HandleC_Exit);
        }

        public ArraySegment<byte> MakeS_Enter(bool success)
        {
            PacketS_Enter sEnter = new PacketS_Enter();
            sEnter.Success = success;

            return PacketManager.Instance.MakePacket(EPacketType.S_Enter, sEnter);
        }

        private void HandleC_Enter(PacketSession session, MemoryStream payloadStream)
        {
            PacketC_Enter cEnter = ProtoBuf.Serializer.Deserialize<PacketC_Enter>(payloadStream);
            Room.Instance.Enter(cEnter.Name, session);
        }

        public ArraySegment<byte> MakeS_Exit(bool success)
        {
            PacketS_Exit sExit = new PacketS_Exit();
            sExit.Success = success;

            return PacketManager.Instance.MakePacket(EPacketType.S_Exit, sExit);
        }

        private void HandleC_Exit(PacketSession session, MemoryStream payloadStream)
        {
            PacketC_Exit cExit = ProtoBuf.Serializer.Deserialize<PacketC_Exit>(payloadStream);
            Room.Instance.Exit(cExit.Name, session);
        }
    }
}
