using ProtoBuf;

namespace ServerCore.Packet
{
    public enum EPacketType : ushort
    { 
        Msg,
        C_Enter,
        S_Enter,
        C_Exit,
        S_Exit,
    }

    [ProtoContract]
    public class PacketMsg
    {
        [ProtoMember(1)]
        public string Msg { get; set; }
    }

    [ProtoContract]
    public class PacketC_Enter
    {
        [ProtoMember(1)]
        public string Name { get; set; }
    }

    [ProtoContract]
    public class PacketS_Enter
    {
        [ProtoMember(1)]
        public bool Success { get; set; }
    }

    [ProtoContract]
    public class PacketC_Exit
    {
        [ProtoMember(1)]
        public string Name { get; set; }
    }

    [ProtoContract]
    public class PacketS_Exit
    {
        [ProtoMember(1)]
        public bool Success { get; set; }
    }
}
