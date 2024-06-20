using ServerCore.Buffer;
using ServerCore.Network;

namespace ServerCore.Packet
{
    public struct PacketHeader
    {
        public ushort packetType;
        public ushort packetSize;
    }

    public class PacketManager
    {
        public static PacketManager Instance { get; } = new PacketManager();
        public ushort HeaderSize { get; } = sizeof(ushort) + sizeof(ushort);

        private Action<PacketSession, MemoryStream>[] _packetHandlers = new Action<PacketSession, MemoryStream>[ushort.MaxValue];

        private PacketManager() { }

        public void AddHandler(EPacketType packetType, Action<PacketSession, MemoryStream> handler)
        {
            _packetHandlers[(ushort)packetType] = handler;
        }

        public ArraySegment<byte> MakePacket<T>(EPacketType packetType, T payload)
        {
            // payload 직렬화
            MemoryStream payloadStream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(payloadStream, payload);
            byte[] serializedPayload = payloadStream.ToArray();

            // PacketHeader 생성
            PacketHeader packetHeader = new PacketHeader { packetType = (ushort)packetType, packetSize = (ushort)(HeaderSize + serializedPayload.Length) };

            // SendBuffer 할당
            ArraySegment<byte> sendBuffer = SendBufferManager.Instance.BufferSegment(packetHeader.packetSize);
            int offset = 0;

            // SendBuffer로 PacketHeader 복사
            Array.Copy(BitConverter.GetBytes((ushort)packetHeader.packetType), 0, sendBuffer.Array, sendBuffer.Offset + offset, sizeof(ushort));
            offset += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(packetHeader.packetSize), 0, sendBuffer.Array, sendBuffer.Offset + offset, sizeof(ushort));
            offset += sizeof(ushort);

            // SendBuffer로 payload 복사
            Array.Copy(serializedPayload, 0, sendBuffer.Array, sendBuffer.Offset + offset, serializedPayload.Length);

            return sendBuffer;
        }

        public int ProcessPacket(PacketSession session, ArraySegment<byte> packet)
        {
            // PacketHeader Parsing이 가능한 크기인지 체크
            if (packet.Count < HeaderSize)
                return 0;

            // PacketHeader Parsing
            PacketHeader packetHeader = new PacketHeader();
            int offset = 0;
            
            packetHeader.packetType = BitConverter.ToUInt16(packet.Array, packet.Offset);
            offset += sizeof(ushort);
            packetHeader.packetSize = BitConverter.ToUInt16(packet.Array, packet.Offset + offset);
            offset += sizeof(ushort);

            // packet 크기 체크
            if (packet.Count < packetHeader.packetSize)
                return 0;

            // PacketType에 해당하는 Handler 호출. Payload 부분만 전달.
            MemoryStream payloadStream = new MemoryStream(packet.Array, packet.Offset + offset, packetHeader.packetSize - HeaderSize);
            _packetHandlers[packetHeader.packetType](session, payloadStream);

            return packetHeader.packetSize;
        }
    }
}
