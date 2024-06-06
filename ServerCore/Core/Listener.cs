using System.Net;
using System.Net.Sockets;

namespace ServerCore.Core
{
    public class Listener
    {
        Socket _listenSocket;
        Action<Socket> _onAcceptHandler;

        public Listener(IPEndPoint iPEndPoint, Action<Socket> onAcceptHandler, int backLog = 10)
        { 
            // listen socket 초기화
            _listenSocket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(iPEndPoint);
            _listenSocket.Listen(backLog);

            _onAcceptHandler = onAcceptHandler;
        }

        public void StartAccept()
        { 
            // 비동기 Accept를 위한 args 초기화
            SocketAsyncEventArgs listenArgs = new SocketAsyncEventArgs();
            listenArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

            // Accept 등록
            RegisterAccept(listenArgs);
        }

        public void RegisterAccept(SocketAsyncEventArgs listenArgs)
        {
            // args 재사용을 위한 값 초기화
            listenArgs.AcceptSocket = null;

            if (_listenSocket.AcceptAsync(listenArgs) == false)
            {
                // Pending 되지 않고 완료
                OnAcceptCompleted(null, listenArgs);
            }
        }

        public void OnAcceptCompleted(object sender, SocketAsyncEventArgs listenArgs) 
        {
            if (listenArgs.SocketError == SocketError.Success)
            {
                // 접속된 Client의 소켓 전달
                _onAcceptHandler(listenArgs.AcceptSocket);
            }
            else
            {
                Console.WriteLine($"Listener Error : {listenArgs.SocketError}");
            }

            // 다시 Accept 시작. args 재사용
            RegisterAccept(listenArgs);
        }
    }
}
