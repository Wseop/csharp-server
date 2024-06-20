using ServerCore.Core;
using System.Net;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientPacketHandler.Instance.Init();

            string host = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(host);
            IPAddress ipAddress = hostEntry.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 7777);

            GameSession session = null;

            Connector connector = new Connector((socket) => {
                session = new GameSession(socket);
                session.Start();

                GameInstance.Instance.Session = session;
            });
            connector.Connect(ipEndPoint);

            // 접속 대기
            while (session == null) ;

            while (true)
            {
                // 입장
                GameInstance.Instance.Enter();

                // 입장 성공 대기
                while (session.Entered == false) ;

                Thread.Sleep(5000);

                // 퇴장
                GameInstance.Instance.Exit();

                // 퇴장 성공 대기
                while (session.Entered == true) ;

                Thread.Sleep(5000);
            }
        }
    }
}
