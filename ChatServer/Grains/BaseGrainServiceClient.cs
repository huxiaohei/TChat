/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 11:22:35
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Grains;
using Abstractions.Message;
using Orleans.Runtime.Services;

namespace ChatServer.Grains
{
    public class BaseGrainServiceClient(IServiceProvider provider, ILocalSiloDetails localSiloDetails)
        : GrainServiceClient<IBaseGrainService>(provider), IBaseGrainServiceClient
    {
        public IServiceProvider Provider { get; } = provider;
        public SiloAddress SiloAddress { get; } = localSiloDetails.SiloAddress;

        public async Task SendMessageAsync(SiloAddress siloAddress, long sessionId, ISCMessage message)
        {
            await GetGrainService(siloAddress).SendMessageAsync(sessionId, message);
        }

        public async Task CloseSessionAsync(SiloAddress siloAddress, long sessionId)
        {
            await GetGrainService(siloAddress).CloseSessionAsync(sessionId);
        }
    }
}