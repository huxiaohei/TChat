/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 16:46:18
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using System.Buffers;

namespace Network.Message
{
    public struct RentBuffer(int size) : IDisposable
    {
        public ArraySegment<byte> Buffer { get; private set; } = new ArraySegment<byte>(ArrayPool<byte>.Shared.Rent(size), 0, size);

        public readonly void Dispose()
        {
            if (Buffer.Array != null)
            {
                ArrayPool<byte>.Shared.Return(Buffer.Array);
            }
        }
    }
}