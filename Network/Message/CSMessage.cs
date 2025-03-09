/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/25 10:02:19
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using System.Buffers;
using System.Text;
using Google.Protobuf;
using Network.Extensions;
using Abstractions.Message;

namespace Network.Message
{
    [Immutable]
    [GenerateSerializer]
    public class CSMessage : ICSMessage
    {
        [Id(0)]
        public long RoleId { get; }
        [Id(1)]
        public uint ClientSerialId { get; }
        [Id(2)]
        public uint ServerSerialId { get; }
        [Id(3)]
        public string MsgName { get; }
        [Id(4)]
        public IMessage Message { get; }

        public CSMessage(long roleId, uint clientSerialId, uint serverSerialId, IMessage message)
        {
            RoleId = roleId;
            ClientSerialId = clientSerialId;
            ServerSerialId = serverSerialId;
            MsgName = message.Descriptor.Name;
            Message = message;
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

        public static CSMessage Decode(byte[] bytes)
        {
            var roleId = BitConverter.ToInt64(bytes);
            var clientSerialId = BitConverter.ToUInt32(bytes, 8);
            var serverSerialId = BitConverter.ToUInt32(bytes, 12);
            var msgNameLength = BitConverter.ToInt32(bytes, 16);
            var msgName = Encoding.UTF8.GetString(bytes, 20, msgNameLength);
            var message = bytes.DecodeMessage(msgName, 20 + msgNameLength);
            if (message == null)
            {
                throw new Exception($"CSMessage Decode message error, Bytes:{bytes}");
            }
            return new CSMessage(roleId, clientSerialId, serverSerialId, message);
        }

    }

}