using ProtoBuf;

namespace ServerCore.Packet
{
    public enum EPacketType : ushort
    { 
        Test,
    }

    [ProtoContract]
    public class PacketTest
    {
        [ProtoMember(1)]
        public string Msg { get; set; }
    }
}
