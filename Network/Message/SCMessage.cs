﻿/*****************************************************************
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
    public class SCMessage(long roleId, uint clientSerialId, uint serverSerialId, IMessage message, bool isCache = false) : ISCMessage
    {
        [Id(0)]
        public long RoleId { get; } = roleId;

        [Id(1)]
        public bool IsCache { get; set; } = isCache;

        [Id(2)]
        public string MsgName { get; } = message.Descriptor.Name;

        [Id(3)]
        public uint ClientSerialId { get; } = clientSerialId;

        [Id(4)]
        public uint ServerSerialId { get; } = serverSerialId;

        [Id(5)]
        public IMessage Message { get; } = message;

        public static SCMessage Decode(byte[] bytes)
        {
            var roleId = BitConverter.ToInt64(bytes);
            var isCache = BitConverter.ToBoolean(bytes, 8);
            var clientSerialId = BitConverter.ToUInt32(bytes, 9);
            var serverSerialId = BitConverter.ToUInt32(bytes, 13);
            var msgNameLength = BitConverter.ToInt32(bytes, 17);
            var msgName = Encoding.UTF8.GetString(bytes, 21, msgNameLength);
            var message = bytes.DecodeMessage(msgName, 21 + msgNameLength) ?? throw new Exception($"SCMessage Decode message error, Bytes:{bytes}");
            return new SCMessage(roleId, clientSerialId, serverSerialId, message, isCache);
        }

        public byte[] Encode()
        {
            var writer = new ArrayBufferWriter<byte>();
            writer.Write(BitConverter.GetBytes(RoleId));
            writer.Write(BitConverter.GetBytes(IsCache));
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