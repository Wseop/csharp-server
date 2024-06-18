using ServerCore.Network;
using System.Net.Sockets;

namespace Server
{
    class SessionManager
    {
        public static SessionManager Instance { get; } = new SessionManager();

        object _lock = new object();
        Dictionary<int, PacketSession> _sessions = new Dictionary<int, PacketSession>();
        int _sessionId = 0;

        private SessionManager()
        { }

        public PacketSession CreateSession(Socket socket)
        {
            lock (_lock)
            {
                PacketSession session = new PacketSession(socket);
                session.SessionId = ++_sessionId;
                _sessions.Add(session.SessionId, session);
                return session;
            }
        }

        public PacketSession GetSession(int sessionId)
        { 
            lock (_lock)
            {
                PacketSession session = null;
                _sessions.TryGetValue(sessionId, out session);
                return session;
            }
        }

        public void RemoveSession(int sessionId)
        {
            lock (_lock)
            {
                _sessions.Remove(sessionId);
            }
        }
    }
}
