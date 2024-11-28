/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/25 16:10:35
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using System.Text;
using System.Buffers;
using Google.Protobuf;
using TChat.Network.Extensions;
using TChat.Abstractions.Message;

namespace TChat.Network.Message
{
    [Immutable]
    [GenerateSerializer]
    public class SCMessage : IBaseMessage
    {
        [Id(0)]
        public string MsgName { get; }
        [Id(1)]
        public uint ClientSerialId { get; }
        [Id(2)]
        public uint ServerSerialId { get; }
        [Id(3)]
        public IMessage Message { get; }

        public SCMessage(uint clientSerialId, uint serverSerialId, IMessage message)
        {
            MsgName = message.Descriptor.Name;
            ClientSerialId = clientSerialId;
            ServerSerialId = serverSerialId;
            Message = message;
        }

        public static SCMessage Decode(byte[] bytes)
        {
            var clientSerialId = BitConverter.ToUInt32(bytes);
            var serverSerialId = BitConverter.ToUInt32(bytes, 4);
            var msgNameLength = BitConverter.ToInt32(bytes, 8);
            var msgName = Encoding.UTF8.GetString(bytes, 12, msgNameLength);
            var message = bytes.DecodeMessage(msgName, 12 + msgNameLength);
            if (message == null)
            {
                throw new Exception($"SCMessage Decode message error, Bytes:{bytes}");
            }
            return new SCMessage(clientSerialId, serverSerialId, message);
        }

        public byte[] Encode()
        {
            var writer = new ArrayBufferWriter<byte>();
            writer.Write(BitConverter.GetBytes(ClientSerialId));
            writer.Write(BitConverter.GetBytes(ServerSerialId));
            var msgNameBytes = Encoding.UTF8.GetBytes(MsgName);
            var msgNameLength = msgNameBytes.Length;
            writer.Write(BitConverter.GetBytes(msgNameLength));
            writer.Write(msgNameBytes);
            writer.Write(Message.ToByteArray());
            return writer.WrittenSpan.ToArray();
        }
    }
}