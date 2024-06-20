using ServerCore.Job;
using ServerCore.Network;

namespace Server
{
    class Room
    {
        public static Room Instance { get; } = new Room();

        JobQueue _jobQueue = new JobQueue();
        Dictionary<string, PacketSession> _sessions = new Dictionary<string, PacketSession>();

        private Room()
        { }

        public void Enter(string name, PacketSession session)
        {
            _jobQueue.PushJob(() => 
            {
                if (_sessions.TryAdd(name, session))
                {
                    // 입장 성공
                    session.Send(ServerPacketHandler.Instance.MakeS_Enter(true));

                    Console.WriteLine($"[{name}] Enter.");
                }
                else
                {
                    // 이미 존재하는 이름. 입장 실패
                    session.Send(ServerPacketHandler.Instance.MakeS_Enter(false));
                }
            });
        }

        public void Exit(string name, PacketSession session)
        {
            _jobQueue.PushJob(() =>
            {
                PacketSession value = null;

                if (_sessions.TryGetValue(name, out value) && value == session)
                {
                    _sessions.Remove(name);
                    session.Send(ServerPacketHandler.Instance.MakeS_Exit(true));

                    Console.WriteLine($"[{name}] Exit.");
                }
                else
                {
                    session.Send(ServerPacketHandler.Instance.MakeS_Exit(false));
                }
            });
        }
    }
}
