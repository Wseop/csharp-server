namespace ServerCore.Buffer
{
    public class SendBufferManager
    {
        // Thread마다 각자의 SendBuffer를 소유
        ThreadLocal<SendBuffer> _sendBuffer = new ThreadLocal<SendBuffer>(() => { return null; });
        readonly int _chunkSize = 0x1000;

        public static SendBufferManager Instance { get; } = new SendBufferManager();

        public ArraySegment<byte> BufferSegment(int bufferSize)
        {
            // 버퍼가 할당되어있지 않으면 새로 생성
            if (_sendBuffer.Value == null)
            {
                _sendBuffer.Value = new SendBuffer(_chunkSize);
            }

            // 버퍼 크기가 부족하면 새로 생성
            if (_sendBuffer.Value.FreeSize() < bufferSize)
            {
                _sendBuffer.Value = new SendBuffer(_chunkSize);
            }

            // bufferSize만큼의 ArraySegment를 반환
            return _sendBuffer.Value.BufferSegment(bufferSize);
        }
    }

    // 지정된 크기의 Buffer Chunk를 생성해두고 필요한만큼 떼어다가 사용
    class SendBuffer
    {
        ArraySegment<byte> _sendBufferChunk;
        int _cursor = 0;

        public SendBuffer(int chunkSize)
        {
            _sendBufferChunk = new ArraySegment<byte>(new byte[chunkSize]);
        }

        // 사용가능한 버퍼의 크기를 반환
        public int FreeSize()
        {
            return _sendBufferChunk.Count - _cursor;
        }

        // bufferSize만큼의 ArraySegment를 반환
        public ArraySegment<byte> BufferSegment(int bufferSize)
        {
            // 버퍼 크기가 부족하면 null을 반환
            if (bufferSize > FreeSize())
                return null;

            return _sendBufferChunk.Slice(_cursor, bufferSize);
        }
    }
}
