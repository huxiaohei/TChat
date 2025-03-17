/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:09:37
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Grains;
using Abstractions.Message;
using ChatServer.Extensions;
using Google.Protobuf;
using Network.Extensions;
using Network.Message;
using Network.Protos;
using Utils.Container;
using Utils.LoggerUtil;

namespace ChatServer.Grains
{
    public class PlayerGrain : Grain, IPlayerGrain
    {
        public readonly long RoleId;
        private readonly IBaseGrainServiceClient _client;
        private SiloAddress? _siloAddress;

        private long _sessionId;
        private uint _serverMsgSerialId = 0;
        private uint _clientMsgSerialId = 0;

        private readonly FixedQueue<ISCMessage> _cacheMessageQueue = new(100);

        public PlayerGrain(IGrainContext grainContext, IGrainRuntime grainRuntime, IBaseGrainServiceClient client)
            : base(grainContext, grainRuntime)
        {
            RoleId = this.GetPrimaryKeyLong();
            _client = client;
            _serverMsgSerialId = 0;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            Loggers.Chat.Info($"PlayerGrain {RoleId} activated");
            await base.OnActivateAsync(cancellationToken);
            // await LoadAsync();
        }

        private void EnqueueCacheMessage(SCMessage resp)
        {
            if (resp.Message.GetType() == typeof(SCErrResp))
            {
                _cacheMessageQueue.Enqueue(resp);
            }
        }

        public async Task SendMessageAsync(IMessage message)
        {
            if (_siloAddress == null)
            {
                return;
            }
            var msg = new SCMessage(RoleId, _clientMsgSerialId, ++_serverMsgSerialId, message);
            EnqueueCacheMessage(msg);
            await _client.SendMessageAsync(_siloAddress, _sessionId, msg);
        }

        public async Task<ISCMessage?> ProcessMessageAsync(SiloAddress siloAddress, long sessionId, ICSMessage message)
        {
            if (_siloAddress == null)
            {
                _siloAddress = siloAddress;
                _sessionId = sessionId;
            }
            else
            {
                if (siloAddress != _siloAddress || sessionId != _sessionId)
                {
                    Loggers.Chat.Error($"PlayerGrain {RoleId} received message from wrong session {siloAddress} {sessionId}");
                    await _client.CloseSessionAsync(_siloAddress, _sessionId);
                }
                _siloAddress = siloAddress;
                _sessionId = sessionId;
            }
            if (message.ClientSerialId <= _clientMsgSerialId)
            {
                Loggers.Chat.Warn($"PlayerGrain {RoleId} received duplicate message {message.ClientSerialId}");
                return default;
            }

            MessageExtension.InitMessageHandlers();
            if (!MessageExtension.MessageHandlers.TryGetValue(message.Message.GetType(), out var handler))
            {
                Loggers.Chat.Warn($"PlayerGrain {RoleId} received unknown message {message.MsgName}");
                return default;
            }
            try
            {
                var rst = await handler(this, message);
                if (rst == null)
                {
                    return default;
                }
                var resp = new SCMessage(
                        RoleId,
                        message.ClientSerialId,
                        ++_serverMsgSerialId,
                        rst);
                EnqueueCacheMessage(resp);
                return resp;
            }
            catch (Exception ex)
            {
                Loggers.Chat.Error($"PlayerGrain process message:{message.MsgName} roleId:{RoleId} error:{ex}");
                return ErrCode.InternalError.Msg(RoleId, message.ClientSerialId, ++_serverMsgSerialId);
            }
        }
    }
}