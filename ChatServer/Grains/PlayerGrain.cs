/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:09:37
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using TChat.Utils.Log;
using Google.Protobuf;
using TChat.Network.Message;
using TChat.Abstractions.Grains;
using TChat.Abstractions.Message;

namespace TChat.ChatServer.Grains
{

    public class PlayerGrain : Grain, IPlayerGrain
    {
        public readonly long RoleId;
        public readonly IBaseGrainServiceClient ServiceClient;
        private SiloAddress? SessionSiloAddress;
        private long SessionId;

        public PlayerGrain(IGrainContext grainContext, IGrainRuntime grainRuntime, IBaseGrainServiceClient serviceClient)
            : base(grainContext, grainRuntime)
        {
            RoleId = this.GetPrimaryKeyLong();
            ServiceClient = serviceClient;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            Loggers.Player.Info($"PlayerGrain {RoleId} activated");
            await base.OnActivateAsync(cancellationToken);
        }

        public async Task<(uint, IMessage?)> ProcessMessage(SiloAddress siloAddress, long sessionId, ICSMessage message)
        {
            if (SessionSiloAddress == null)
            {
                SessionSiloAddress = siloAddress;
                SessionId = sessionId;
            }
            else
            {
                if (siloAddress != SessionSiloAddress || sessionId != SessionId)
                {

                }
            }
            Loggers.Player.Info($"PlayerGrain {RoleId} received message {message}");
            await ServiceClient.SendMessageAsync(siloAddress, sessionId, new SCMessage(0, 0, message.Message));
            return default;
        }
    }

}