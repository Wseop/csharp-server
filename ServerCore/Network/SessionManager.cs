namespace ServerCore.Network
{
    public class SessionManager
    {
        public static SessionManager Instance { get; } = new SessionManager();

        object _lock = new object();
        private Func<Session> _sessionFactory = null;
        Dictionary<int, Session> _sessions = new Dictionary<int, Session>();
        int _sessionId = 0;

        private SessionManager()
        { }

        public void SetSessionFactory(Func<Session> sessionFactory)
        { 
            _sessionFactory = sessionFactory;
        }

        public Session CreateSession()
        {
            if (_sessionFactory == null)
                return null;

            lock (_lock)
            {
                Session session = _sessionFactory();
                session.SessionId = ++_sessionId;
                _sessions.Add(session.SessionId, session);
                return session;
            }
        }

        public Session GetSession(int sessionId)
        {
            lock (_lock)
            {
                Session session = null;
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
