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
using Network.Message;
using Network.Protos;
using Utils.LoggerUtil;
using Utils.TimeUtil;

namespace ChatServer.Grains
{
    public class PlayerGrain : Grain, IPlayerGrain
    {
        public readonly long RoleId;
        private readonly IBaseGrainServiceClient _client;
        private SiloAddress? _siloAddress;
        private long _sessionId;
        private uint _msgId;


        public PlayerGrain(IGrainContext grainContext, IGrainRuntime grainRuntime, IBaseGrainServiceClient client)
            : base(grainContext, grainRuntime)
        {
            RoleId = this.GetPrimaryKeyLong();
            _client = client;
            _msgId = 0;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            Loggers.Chat.Info($"PlayerGrain {RoleId} activated");
            await base.OnActivateAsync(cancellationToken);
        }

        public async Task<ISCMessage?> ProcessMessage(SiloAddress siloAddress, long sessionId, ICSMessage message)
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
            if (message.MsgName == "CSPing")
            {
                return new SCMessage(message.ClientSerialId, ++_msgId, new SCPong() { ClientTimeMs = message.ClientSerialId, ServerTimeMs = Time.NowMilliseconds() });
            }
            // Loggers.Chat.Info($"PlayerGrain {RoleId} received message {message}");
            // await _client.SendMessageAsync(siloAddress, sessionId, new SCMessage(0, 0, message.Message));
            return new SCMessage(message.ClientSerialId, ++_msgId, ErrCode.Ok.Msg());
        }
    }
}