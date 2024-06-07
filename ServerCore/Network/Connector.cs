using System.Net;
using System.Net.Sockets;

namespace ServerCore.Core
{
    public class Connector
    {
        Action<Socket> _onConnectHandler;

        public Connector(Action<Socket> onConnectHandler)
        { 
            _onConnectHandler = onConnectHandler;
        }

        public void Connect(IPEndPoint ipEndPoint)
        {
            Socket socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
            connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
            connectArgs.RemoteEndPoint = ipEndPoint;
            connectArgs.UserToken = socket;

            RegisterConnect(socket, connectArgs);
        }

        void RegisterConnect(Socket socket, SocketAsyncEventArgs connectArgs)
        {
            if (socket.ConnectAsync(connectArgs) == false)
            {
                OnConnectCompleted(null, connectArgs);
            }
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs connectArgs)
        {
            if (connectArgs.SocketError == SocketError.Success)
            {
                _onConnectHandler(connectArgs.ConnectSocket);
            }
            else
            {
                Console.WriteLine($"Connector Error : {connectArgs.SocketError}");
            }
        }
    }
}
