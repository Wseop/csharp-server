using ServerCore.Network;
using System.Net.Sockets;

namespace DummyClient
{
    class GameSession : PacketSession
    {
        public string Name { get; set; }
        public bool Entered { get; set; } = false;

        public GameSession(Socket socket) : base(socket) { }
    }
}
