/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/25 10:02:19
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Message;
using Google.Protobuf;
using Network.Extensions;
using System.Buffers;
using System.Text;

namespace Network.Message
{
    [Immutable]
    [GenerateSerializer]
    [Alias("Network.Message.CSMessage")]
    public class CSMessage(long roleId, uint clientSerialId, uint serverSerialId, IMessage message) : ICSMessage
    {
        [Id(0)]
        public long RoleId { get; } = roleId;

        [Id(1)]
        public uint ClientSerialId { get; } = clientSerialId;

        [Id(2)]
        public uint ServerSerialId { get; } = serverSerialId;

        [Id(3)]
        public string MsgName { get; } = message.Descriptor.Name;

        [Id(4)]
        public IMessage Message { get; } = message;

        public byte[] Encode()
        {
            var writer = new ArrayBufferWriter<byte>();
            writer.Write(BitConverter.GetBytes(RoleId));
            writer.Write(BitConverter.GetBytes(ClientSerialId));
            writer.Write(BitConverter.GetBytes(ServerSerialId));
            var msgNameBytes = Encoding.UTF8.GetBytes(MsgName);
            var msgNameLength = msgNameBytes.Length;
            writer.Write(BitConverter.GetBytes(msgNameLength));
            writer.Write(msgNameBytes);
            writer.Write(Message.ToByteArray());
            return writer.WrittenSpan.ToArray();
        }

        public static CSMessage Decode(byte[] bytes)
        {
            var roleId = BitConverter.ToInt64(bytes);
            var clientSerialId = BitConverter.ToUInt32(bytes, 8);
            var serverSerialId = BitConverter.ToUInt32(bytes, 12);
            var msgNameLength = BitConverter.ToInt32(bytes, 16);
            var msgName = Encoding.UTF8.GetString(bytes, 20, msgNameLength);
            var message = bytes.DecodeMessage(msgName, 20 + msgNameLength) ?? throw new Exception($"CSMessage Decode message error, Bytes:{bytes}");
            return new CSMessage(roleId, clientSerialId, serverSerialId, message);
        }
    }
}