/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:09:37
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using TChat.Utils.Log;
using TChat.Abstractions.Grains;
using TChat.Abstractions.Message;

namespace TChat.ChatServer.Grains
{

    public class PlayerGrain : Grain, IPlayerGrain
    {
        public readonly long RoleId;

        public PlayerGrain(IGrainContext grainContext, IGrainRuntime grainRuntime)
            : base(grainContext, grainRuntime)
        {
            RoleId = this.GetPrimaryKeyLong();
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            Loggers.Player.Info($"PlayerGrain {RoleId} activated");
            await base.OnActivateAsync(cancellationToken);
        }

        public Task<ISCMessage?> ProcessMessage(SiloAddress siloAddress, long sessionId, ICSMessage message)
        {
            Loggers.Player.Info($"PlayerGrain {RoleId} received message {message}");
            return Task.FromResult<ISCMessage?>(null);
        }
    }

}