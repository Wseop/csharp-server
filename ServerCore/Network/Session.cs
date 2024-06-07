using ServerCore.Buffer;
using System.Net.Sockets;

namespace ServerCore.Network
{
    public abstract class Session
    {
        Socket _socket;
        int _connected = 1;
        int _started = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(0x1000);

        object _lock = new object();
        bool _sendRegistered = false;
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();

        public Session(Socket socket)
        {
            _socket = socket;
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
        }

        protected abstract void OnDisconnect();
        protected abstract int OnRecv(ArraySegment<byte> buffer);
        protected abstract void OnSend(int bytesTransferred);

        public void Start()
        {
            // 연결이 끊겼거나 이미 시작된 상태이면 빠져나옴
            if (_connected == 0 || _started == 1)
                return;

            _started = 1;

            // Receive 시작
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            RegisterRecv(recvArgs);
        }

        void Disconnect()
        {
            if (Interlocked.Exchange(ref _connected, 0) == 0)
                return;

            OnDisconnect();

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        void RegisterRecv(SocketAsyncEventArgs recvArgs)
        {
            // 수신 Buffer 설정
            ArraySegment<byte> recvBuffer = _recvBuffer.BufferSegment();
            recvArgs.SetBuffer(recvBuffer.Array, recvBuffer.Offset, recvBuffer.Count);

            // Receive
            if (_socket.ReceiveAsync(recvArgs) == false)
            {
                OnRecvCompleted(null, recvArgs);
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs recvArgs)
        {
            int bytesReceived = recvArgs.BytesTransferred;

            if (bytesReceived == 0)
            {
                Disconnect();
            }
            else if (recvArgs.SocketError != SocketError.Success)
            {
                Console.WriteLine($"Recv Error : {recvArgs.SocketError}");

                Disconnect();
            }
            else
            {
                // 받은 데이터의 크기만큼 RecvBuffer 커서 이동
                if (_recvBuffer.OnWrite(bytesReceived) == false)
                {
                    Disconnect();
                    return;
                }

                // 받은 데이터 처리, 처리한 데이터의 크기만큼 RecvBuffer 커서 이동
                int bytesProcessed = OnRecv(_recvBuffer.DataSegment());
                if (bytesProcessed > bytesReceived || _recvBuffer.OnRead(bytesProcessed) == false)
                {
                    Disconnect();
                    return;
                }

                // Receive 재개
                RegisterRecv(recvArgs);
            }
        }

        public void Send(ArraySegment<byte> sendBuffer)
        {
            bool registerSend = false;
            
            lock (_lock)
            {
                // 보낼 데이터를 Queue에 모음
                _sendQueue.Enqueue(sendBuffer);

                // Send를 처리중인 Thread가 없다면 현재 Thread가 담당
                if (_sendRegistered == false)
                {
                    _sendRegistered = true;
                    registerSend = true;
                }
            }

            if (registerSend)
            {
                RegisterSend();
            }
        }

        public void Send(List<ArraySegment<byte>> sendBuffers)
        {
            bool registerSend = false;

            lock (_lock)
            { 
                foreach (ArraySegment<byte> sendBuffer in sendBuffers) 
                {
                    _sendQueue.Enqueue(sendBuffer);
                }

                if (_sendRegistered == false)
                {
                    _sendRegistered = true;
                    registerSend = true;
                }
            }

            if (registerSend)
            {
                RegisterSend();
            }
        }

        void RegisterSend()
        {
            List<ArraySegment<byte>> bufferList = new List<ArraySegment<byte>>();

            // 보낼 데이터를 모두 꺼내서 args에 등록
            lock (_lock)
            {
                while (_sendQueue.Count > 0)
                {
                    bufferList.Add(_sendQueue.Dequeue());
                }
            }
            _sendArgs.BufferList = bufferList;

            if (_socket.SendAsync(_sendArgs) == false)
            {
                OnSendCompleted(null, _sendArgs);
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs sendArgs)
        {
            int bytesTransferred = sendArgs.BytesTransferred;

            if (bytesTransferred == 0)
            {
                Disconnect();
            }
            else if (sendArgs.SocketError != SocketError.Success)
            {
                Console.WriteLine($"Send Error : {sendArgs.SocketError}");

                Disconnect();
            }
            else
            {
                OnSend(bytesTransferred);

                bool registerSend = false;

                lock (_lock)
                {
                    // 보낼 데이터가 더 쌓인경우 현재 Thread가 전송 처리
                    if (_sendQueue.Count > 0)
                    {
                        registerSend = true;
                    }
                    else
                    {
                        _sendRegistered = false;
                    }
                }

                if (registerSend)
                {
                    RegisterSend();
                }
            }
        }
    }
}
