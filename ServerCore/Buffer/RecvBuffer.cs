namespace ServerCore.Buffer
{
    public class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos = 0;
        int _writePos = 0;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize]);
        }

        // 수신된 데이터의 크기를 반환
        public int DataSize()
        {
            return _writePos - _readPos;
        }

        // 사용가능한 버퍼의 크기를 반환
        public int FreeSize()
        {
            return _buffer.Count - _writePos;
        }

        // 수신된 데이터가 있는 ArraySegment를 반환
        public ArraySegment<byte> DataSegment()
        {
            return _buffer.Slice(_readPos, DataSize());
        }

        // 사용가능한 ArraySegment를 반환
        public ArraySegment<byte> BufferSegment()
        {
            return _buffer.Slice(_writePos, FreeSize());
        }

        // 처리한 데이터의 크기만큼 readPos 이동
        public bool OnRead(int bytesProcessed)
        {
            if (bytesProcessed > DataSize())
                return false;

            _readPos += bytesProcessed;
            return true;
        }

        // 수신된 데이터의 크기만큼 writePos 이동
        public bool OnWrite(int bytesReceived)
        {
            if (bytesReceived > FreeSize())
                return false;

            _writePos += bytesReceived;
            return true;
        }

        // 버퍼를 재사용하기 위해 커서를 초기화
        public void Clean()
        {
            int dataSize = DataSize();

            // 처리할 데이터가 없으면 0으로 초기화
            if (dataSize == 0)
            {
                _readPos = 0;
                _writePos = 0;
            }
            // 처리할 데이터가 남아있으면, 남아있는 데이터도 앞쪽으로 땡겨옴
            else
            {
                Array.Copy(DataSegment().Array, _buffer.Array, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }
    }
}
