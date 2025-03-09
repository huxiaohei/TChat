/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 16:46:18
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using System.Buffers;

namespace Network.Message
{
    public struct RentBuffer : IDisposable
    {
        public ArraySegment<byte> Buffer { get; private set; }

        public RentBuffer(int size)
        {
            Buffer = new ArraySegment<byte>(ArrayPool<byte>.Shared.Rent(size), 0, size);
        }

        public void Dispose()
        {
            if (Buffer.Array != null)
            {
                ArrayPool<byte>.Shared.Return(Buffer.Array);
            }
        }
    }
}