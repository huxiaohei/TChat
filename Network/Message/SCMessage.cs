/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/25 16:10:35
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
    [Alias("Network.Message.SCMessage")]
    public class SCMessage : ISCMessage
    {
        [Id(0)]
        public long RoleId { get; }

        [Id(1)]
        public string MsgName { get; }

        [Id(2)]
        public uint ClientSerialId { get; }

        [Id(3)]
        public uint ServerSerialId { get; }

        [Id(4)]
        public IMessage Message { get; }

        public SCMessage(long roleId, uint clientSerialId, uint serverSerialId, IMessage message)
        {
            RoleId = roleId;
            MsgName = message.Descriptor.Name;
            ClientSerialId = clientSerialId;
            ServerSerialId = serverSerialId;
            Message = message;
        }

        public static SCMessage Decode(byte[] bytes)
        {
            var roleId = BitConverter.ToInt64(bytes);
            var clientSerialId = BitConverter.ToUInt32(bytes, 8);
            var serverSerialId = BitConverter.ToUInt32(bytes, 12);
            var msgNameLength = BitConverter.ToInt32(bytes, 16);
            var msgName = Encoding.UTF8.GetString(bytes, 20, msgNameLength);
            var message = bytes.DecodeMessage(msgName, 20 + msgNameLength);
            if (message == null)
            {
                throw new Exception($"SCMessage Decode message error, Bytes:{bytes}");
            }
            return new SCMessage(roleId, clientSerialId, serverSerialId, message);
        }

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
    }
}